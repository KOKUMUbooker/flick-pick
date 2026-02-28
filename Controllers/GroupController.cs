namespace WatchHive.Controllers;

using Microsoft.AspNetCore.Mvc;
using WatchHive.DTOs;
using WatchHive.Models;

[ApiController]
[Route("/api/groups/")]
public class GroupController : ControllerBase
{
    private readonly WatchHiveDbContext _dbContext;

    public GroupController(WatchHiveDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("{userId}")]
    public IActionResult GetGroups([FromQuery] string userId) 
    {
        return Ok("Get groups");
    }

    [HttpPost("")]
    public IActionResult CreateGroup([FromBody] CreateGroupDto groupDto) 
    {
        return Ok("Get groups");
    }

    [HttpDelete("/{id}/leave")]
    public IActionResult ExitGroup(string groupId) 
    {
        return Ok("Exit group");
    } 

    [HttpPatch("/{id}/members/{userId}")]
    public IActionResult ChangeMemberRole(string groupId,string userId) 
    {
        return Ok("Change member roles");
    }      
}