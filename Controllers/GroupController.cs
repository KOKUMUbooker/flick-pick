namespace WatchHive.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using WatchHive.DTOs;
using WatchHive.Models;

[Authorize]
[ApiController]
[Route("/api/")]
public class GroupController : ControllerBase
{
    private readonly WatchHiveDbContext _dbContext;

    public GroupController(WatchHiveDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("groups")]
    public async Task<IActionResult> GetGroups([FromQuery] string userId) 
    {
        if (!Guid.TryParse(userId, out var parsedUserId))
        {
            return BadRequest(new CustomError { Message = "Invalid userId provided"});
        }

        var groups = await _dbContext.Groups.
                    Where(g => g.Members.Any(m => m.UserId == parsedUserId))
                    .Select(g => new  
                    {
                        Id = g.Id,
                        Name = g.Name,
                        Description = g.Description,
                        MembersCount = g.Members.Count,
                        IsUserAdmin = g.Members.Any(m => m.IsAdmin && m.UserId == parsedUserId)
                    })
                    .AsNoTracking()
                    .ToListAsync();

        return Ok(new { groups });
    }

    [HttpPost("groups")]
    public async Task<IActionResult> UpsertGroup([FromBody] UpsertGroupDto groupDto) 
    {
        if (!Guid.TryParse(groupDto.UserId, out Guid parsedUserId))
        {
            return BadRequest(new CustomError { Message = "Invalid userId provided" });
        } 

        var userIdExists = await _dbContext.Users.FindAsync(parsedUserId);
        if (userIdExists == null)
        {
            return BadRequest(new CustomError { Message = "Request initiated by a user that does not exist" });
        }

        // If Id is present in dto, user wants to update
        if (!string.IsNullOrEmpty(groupDto.Id) && !string.IsNullOrWhiteSpace(groupDto.Id))
        {
            if (!Guid.TryParse(groupDto.Id, out Guid parsedGroupId))
            {
                return BadRequest(new CustomError { Message = "Invalid groupId provided" });
            } 

            var group = await _dbContext.Groups.FindAsync(parsedGroupId);
            if (group == null)
            {
                return BadRequest(new CustomError{ Message = "Group does not exist" });
            }

            var newName = groupDto.Name.Trim();
            if (group.Name != newName)
            {
                var sameNamedGroup = await _dbContext.Groups
                        .AnyAsync(g => g.Name == newName && g.CreatedById == parsedUserId);

                if (sameNamedGroup)
                {
                    return BadRequest(new CustomError { Message = "You already have a group with that name" });
                }
            }
            
            group.Name = groupDto.Name.Trim() ?? group.Name;
            group.Description = groupDto.Description?.Trim() ?? group.Description;
 
            await _dbContext.SaveChangesAsync();
            return Ok(new { message = "Group updated successfully", group = group.Id });
        }

        // Check for similarly named group
        var groupName = groupDto.Name.Trim();
        var groupExists = await _dbContext.Groups
            .AnyAsync(g => g.Name == groupName && g.CreatedById == parsedUserId);

        if (groupExists)
        {
            return BadRequest(new CustomError { Message = "You already have a group with that name" });
        }
        var newGroup = new Group()
        {
            Name = groupName,
            CreatedById = parsedUserId,
            Description = groupDto.Description
        };
        await _dbContext.Groups.AddAsync(newGroup);

        // Add creator as a member(admin) of the group
        var groupMember = new UserGroup()
        {
            GroupId = newGroup.Id,
            IsAdmin = true,
            UserId = parsedUserId
        };
        await _dbContext.UserGroups.AddAsync(groupMember);

        await _dbContext.SaveChangesAsync(); 

        return Ok(new { message = "Group created successfully", group = newGroup.Id });
    }

    [HttpDelete("groups/{groupId}/del")]
    public async Task<IActionResult> DeleteGroup(Guid groupId, [FromQuery] Guid userId)
    {
        var group = await _dbContext.Groups.FindAsync(groupId);
        var message = "Group deleted successfully";
        if (group == null)
        {
            return Ok(new { message });
        }
        
        // Only allow group creator to delete the group
        if (userId != group.CreatedById)
        {
            return BadRequest(new CustomError{Message = "You're not allowed to delete this group"});
        }

        await _dbContext.Groups
                .Where(g => g.Id == groupId && g.CreatedById == userId)
                .ExecuteDeleteAsync();

        return Ok(new { message });
    }

    [HttpDelete("groups/{groupId}/leave")]
    public async Task<IActionResult> RemoveMemberFromGroup(
        [FromRoute] Guid groupId, 
        [FromQuery] Guid userId,
        [FromQuery] Guid initiator
    ) 
    {
        if (userId == initiator)
        {
            return BadRequest(new CustomError{Message="You can't remove yourself from the group"});
        }

        // Ensure user is a group admin before proceeding
        var isGroupAdmin = await _dbContext.UserGroups.
                            Where(ug => ug.GroupId == groupId && ug.UserId == initiator && ug.IsAdmin)
                            .AsNoTracking()
                            .FirstAsync();
        if (isGroupAdmin == null)
        {
            return BadRequest(new CustomError {Message="You are not allowed to remove a user from this group"} );
        }

        var groupExists = await _dbContext.Groups.FindAsync(groupId);
        if (groupExists == null)
        {
            return BadRequest(new CustomError { Message = "Group for the provided groupId does not exist" });
        }

        await _dbContext.UserGroups
            .Where(ug => ug.GroupId == groupId && ug.UserId == userId)
            .ExecuteDeleteAsync();
        
        return Ok(new {message = "User successfully removed the group" });
    } 

    [HttpPatch("groups/{groupId}/members/{memberUserId}")]
    public async Task<IActionResult> ChangeMemberRole(
        [FromRoute] Guid groupId, 
        [FromRoute] Guid memberUserId, 
        [FromQuery] Guid initiator,
        [FromBody] ChangeGroupMemberRoleDto roleChangeDto
    ) 
    {
        // Ensure user is a group admin before proceeding
        var isGroupAdmin = await _dbContext.UserGroups.
                            Where(ug => ug.GroupId == groupId && ug.UserId == initiator && ug.IsAdmin)
                            .AsNoTracking()
                            .FirstAsync();
        if (isGroupAdmin == null)
        {
            return BadRequest(new CustomError {Message="You are not allowed to remove a user from this group"} );
        }

        var groupExists = await _dbContext.Groups.FindAsync(groupId);
        if (groupExists == null)
        {
            return BadRequest(new CustomError { Message = "The group does not exist" });
        }

        var groupMember = await _dbContext.UserGroups
            .Where(ug => ug.GroupId == groupId && ug.UserId == memberUserId)
            .FirstAsync();
        
        if (groupMember == null)
        {
            return BadRequest(new CustomError { Message = "Group member does not exist" });
        }

        // You shouldn't be able to revoke role of the group creator 
        if (!roleChangeDto.MakeAdmin && groupExists.CreatedById == memberUserId)
        {
            return BadRequest(new CustomError{Message="You can't demote the creator of the group"});
        }

        groupMember.IsAdmin = roleChangeDto.MakeAdmin;

        await _dbContext.SaveChangesAsync();

        string Message = roleChangeDto.MakeAdmin 
            ? "Member successfully promoted to admin" 
            : "Member successfully stripped of their admin role";

        return Ok(new { Message });
    }      
}