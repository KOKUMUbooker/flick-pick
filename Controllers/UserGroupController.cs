namespace WatchHive.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WatchHive.DTOs;
using WatchHive.Models;

[ApiController]
[Route("/api/groups/")]
public class UserGroupController : ControllerBase
{
    private readonly WatchHiveDbContext _dbContext;

    public UserGroupController(WatchHiveDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("{groupId}/members")]
    public async Task<IActionResult> GetMembers([FromQuery] string userId, [FromRoute] string groupId)
    {
        if (!Guid.TryParse(userId, out var parsedUserId))
        {
            return BadRequest(new CustomError { Message = "Invalid userId provided"});
        }

        if (!Guid.TryParse(groupId, out Guid parsedGroupId))
        {
            return BadRequest(new CustomError { Message = "Invalid group id provided" });
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
                        .ToListAsync();

        return Ok(new { groupMembers } );
    }
}