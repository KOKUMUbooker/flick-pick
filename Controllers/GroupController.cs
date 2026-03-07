namespace WatchHive.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WatchHive.DTOs;
using WatchHive.Models;

[ApiController]
[Route("/api/groups")]
public class GroupController : ControllerBase
{
    private readonly WatchHiveDbContext _dbContext;

    public GroupController(WatchHiveDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetGroups([FromQuery] string userId) 
    {
        if (!Guid.TryParse(userId, out var parsedId))
        {
            return BadRequest(new CustomError { Message = "Invalid userId provided"});
        }

        var groups = await _dbContext.Groups.
                        Where(g => g.CreatedById == parsedId)
                        .AsNoTracking()
                        .ToListAsync();

        return Ok(new { groups });
    }

    [HttpPost]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupDto groupDto) 
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

        // Check for similarly named group
        var sameGroupName = await _dbContext.Groups
                                .Where(g => g.Name == groupDto.Name.Trim() && g.CreatedById == parsedUserId)
                                .FirstAsync();
        if (sameGroupName != null)
        {
             return BadRequest(new CustomError { Message = "You already have a group with that name" });
        }

        var newGroup = new Group()
        {
            Name = groupDto.Name.Trim(),
            CreatedById = parsedUserId,
            Description = groupDto.Description
        };

        await _dbContext.Groups.AddAsync(newGroup);
        await _dbContext.SaveChangesAsync(); 

        return Ok(new { message = "Group created successfully", group = newGroup.Id });
    }

    [HttpDelete("/{groupId}/leave")]
    public async Task<IActionResult> ExitGroup(string groupId, [FromQuery] string userId) 
    {
        if (!Guid.TryParse(groupId, out Guid parsedGroupId))
        {
            return BadRequest(new CustomError { Message = "Invalid group id provided" });
        }

        if (!Guid.TryParse(userId, out Guid parsedUserId))
        {
            return BadRequest(new CustomError { Message = "Invalid user id provided" });
        }

        var groupExists = await _dbContext.Groups.FindAsync(parsedGroupId);
        if (groupExists == null)
        {
            return BadRequest(new CustomError { Message = "Group for the provided groupId does not exist" });
        }

        await _dbContext.UserGroups
            .Where(ug => ug.GroupId == parsedGroupId && ug.UserId == parsedUserId)
            .ExecuteDeleteAsync();

        return Ok(new {message = "User successfully exited the group" });
    } 

    [HttpPatch("/{groupId}/members/{userId}")]
    public async Task<IActionResult> ChangeMemberRole(string groupId, string userId, [FromBody] ChangeGroupMemberRoleDto roleChangeDto) 
    {
        if (!Guid.TryParse(groupId, out Guid parsedGroupId))
        {
            return BadRequest(new CustomError { Message = "Invalid group id provided" });
        }

        if (!Guid.TryParse(userId, out Guid parsedUserId))
        {
            return BadRequest(new CustomError { Message = "Invalid user id provided" });
        }

        var groupExists = await _dbContext.Groups.FindAsync(parsedGroupId);
        if (groupExists == null)
        {
            return BadRequest(new CustomError { Message = "Group for the provided groupId does not exist" });
        }

        var groupMember = await _dbContext.UserGroups
            .Where(ug => ug.GroupId == parsedGroupId && ug.UserId == parsedUserId)
            .FirstAsync();
        
        if (groupMember == null)
        {
            return BadRequest(new CustomError { Message = "Group member does not exist" });
        }

        groupMember.IsAdmin = roleChangeDto.MakeAdmin;

        await _dbContext.SaveChangesAsync();

        string Message = roleChangeDto.MakeAdmin 
            ? "Member successfully promoted to admin" 
            : "Member successfully stripped of their admin role";

        return Ok(new { Message });
    }      
}