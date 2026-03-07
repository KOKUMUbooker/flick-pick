namespace WatchHive.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WatchHive.DTOs;
using WatchHive.Models;

[ApiController]
[Route("/api/")]
public class UserGroupController : ControllerBase
{
    private readonly WatchHiveDbContext _dbContext;

    public UserGroupController(WatchHiveDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("groups/{groupId}/members")]
    public async Task<IActionResult> GetMembers([FromRoute] string groupId, [FromQuery] string userId)
    {
        if (!Guid.TryParse(userId, out var parsedUserId))
        {
            return BadRequest(new CustomError { Message = "Invalid userId provided"});
        }

        if (!Guid.TryParse(groupId, out Guid parsedGroupId))
        {
            return BadRequest(new CustomError { Message = "Invalid group id provided" });
        }

        // Check if group exists
        bool groupExists = await _dbContext.Groups
            .AnyAsync(g => g.Id == parsedGroupId );
        if (!groupExists)
        {
             return BadRequest(new CustomError {Message = "The Group does not exist"});
        }

         // Check whether user is a member in the group they want to fetch its members
        bool isMember = await _dbContext.UserGroups
            .AnyAsync(g => g.GroupId == parsedGroupId && g.UserId == parsedUserId);
        
        if (!isMember)
        {
            return BadRequest(new CustomError {Message = "You are not allowed to view members of a group you're not part of"});
        }

        var groupMembers = await _dbContext.UserGroups
            .Where(ug => ug.GroupId == parsedGroupId)
            .Select(ug => new UserGroupDto
            {
                Id = ug.Id.ToString(),
                IsAdmin = ug.IsAdmin,
                JoinedAt = ug.JoinedAt,
                UserId = ug.UserId.ToString(),
                FullName = ug.User.FullName,
                Email = ug.User.Email
            })
            .ToListAsync();

        return Ok(new { groupMembers } );
    }
}