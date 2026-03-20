namespace WatchHive.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WatchHive.DTOs;
using WatchHive.Models;

[ApiController]
[Route("/api/")]
public class GroupInvitationSearchController : ControllerBase
{
    private readonly WatchHiveDbContext _dbContext;

    public GroupInvitationSearchController(WatchHiveDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("groups/invite/search")]
    public async Task<IActionResult> GetGroupsToJoin(
        [FromQuery] string userId,
        [FromQuery] string query,
        [FromQuery] Guid? cursor,
        [FromQuery] int? limit,
        [FromQuery] string direction = "next" // "next" | "prev"
    )
    {
        if (!Guid.TryParse(userId, out Guid parsedUserId))
        {
            return BadRequest(new CustomError { Message = "Invalid user id provided" });
        }

        var userExists = await _dbContext.Users.AnyAsync(u => u.Id == parsedUserId);
        if (!userExists)
        {
            return NotFound(new CustomError { Message = "The user does not exist" });
        }

        limit ??= 10;

        // Base query (reused everywhere)
        var baseQuery = _dbContext.Groups
            .Where(g => !g.Members.Any(m => m.UserId == parsedUserId));

        if (!string.IsNullOrEmpty(query))
        {
            baseQuery = baseQuery.Where(g =>
                // EF.Functions.Like(g.Name, $"%{query}%") // Doesn't seem to work, investigate later
                g.Name.ToLower().Contains(query.ToLower())
            );
        }

        // Apply cursor filter
        if (cursor.HasValue)
        {
            if (direction == "next")
            {
                baseQuery = baseQuery.Where(g => g.Id > cursor.Value);
            }
            else if (direction == "prev")
            {
                baseQuery = baseQuery.Where(g => g.Id < cursor.Value);
            }
        }

        // Apply ordering
        var orderedQuery = direction == "prev"
            ? baseQuery.OrderByDescending(g => g.Id)
            : baseQuery.OrderBy(g => g.Id);

        var groups = await orderedQuery
            .Take(limit.Value + 1)
            .Select(g => new
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                MemberCount = g.Members.Count,
                CreatorFullName = g.CreatedBy.FullName,
                CreatorEmail = g.CreatedBy.FullName,
                CreatedAt = g.Created
            })
            .AsNoTracking()
            .ToListAsync();

        bool hasMoreInDirection = groups.Count > limit.Value;

        if (hasMoreInDirection)
        {
            groups = groups.Take(limit.Value).ToList();
        }

        // Reverse back if prev
        if (direction == "prev")
        {
            groups.Reverse();
        }

        Guid? nextCursor = null;
        Guid? prevCursor = null;

        if (groups.Count != 0)
        {
            nextCursor = groups.Last().Id;
            prevCursor = groups.First().Id;
        }

        // Accurate hasNext / hasPrev
        bool hasNextPage = false;
        bool hasPrevPage = false;

        if (groups.Count != 0)
        {
            var firstId = groups.First().Id;
            var lastId = groups.Last().Id;

            hasPrevPage = await baseQuery
                .Where(g => g.Id < firstId)
                .AnyAsync();

            hasNextPage = await baseQuery
                .Where(g => g.Id > lastId)
                .AnyAsync();
        }

        return Ok(new
        {
            results = groups,
            nextCursor,
            prevCursor,
            hasNextPage,
            hasPrevPage
        });
    }

    [HttpGet("users/invite/search")]
    public async Task<IActionResult> GetUsersToJoinGroup(
        [FromQuery] string query,
        [FromQuery] Guid groupId,
        [FromQuery] Guid userId,
        [FromQuery] Guid? cursor,
        [FromQuery] int? limit,
        [FromQuery] string direction = "next" // "next" | "prev"
    )
    {
        var userExists = await _dbContext.Users.AnyAsync(u => u.Id == userId);
        if (!userExists)
        {
            return NotFound(new CustomError { Message = "The user does not exist" });
        }

        var groupExists = await _dbContext.Groups.AnyAsync(g => g.Id == groupId);
        if (!groupExists)
        {
            return NotFound(new CustomError { Message = "The group does not exist" });
        }

        limit ??= 10;

        // Base query (users NOT in group)
        var baseQuery = _dbContext.Users
            .Where(u => !u.UserGroups.Any(ug => ug.GroupId == groupId || ug.UserId == userId));

        // Search filter (safe for EF)
        if (!string.IsNullOrEmpty(query))
        {
            baseQuery = baseQuery.Where(u =>
                // EF.Functions.Like(u.FullName, $"%{query}%") ||
                // EF.Functions.Like(u.Email, $"%{query}%")
                
                u.FullName.ToLower().Contains(query.ToLower()) ||
                u.Email.ToLower().Contains(query.ToLower()) 
            );
        }

        // Cursor filter
        if (cursor.HasValue)
        {
            if (direction == "next")
            {
                baseQuery = baseQuery.Where(u => u.Id > cursor.Value);
            }
            else if (direction == "prev")
            {
                baseQuery = baseQuery.Where(u => u.Id < cursor.Value);
            }
        }

        // Ordering
        var orderedQuery = direction == "prev"
            ? baseQuery.OrderByDescending(u => u.Id)
            : baseQuery.OrderBy(u => u.Id);

        var users = await orderedQuery
            .Take(limit.Value + 1)
            .Select(u => new
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email
            })
            .AsNoTracking()
            .ToListAsync();

        bool hasMoreInDirection = users.Count > limit.Value;

        if (hasMoreInDirection)
        {
            users = users.Take(limit.Value).ToList();
        }

        // Reverse back for prev
        if (direction == "prev")
        {
            users.Reverse();
        }

        Guid? nextCursor = null;
        Guid? prevCursor = null;

        if (users.Count != 0)
        {
            nextCursor = users.Last().Id;
            prevCursor = users.First().Id;
        }

        // Accurate pagination flags
        bool hasNextPage = false;
        bool hasPrevPage = false;

        if (users.Count != 0)
        {
            var firstId = users.First().Id;
            var lastId = users.Last().Id;

            hasPrevPage = await baseQuery
                .Where(u => u.Id < firstId)
                .AnyAsync();

            hasNextPage = await baseQuery
                .Where(u => u.Id > lastId)
                .AnyAsync();
        }

        return Ok(new
        {
            results = users,
            nextCursor,
            prevCursor,
            hasNextPage,
            hasPrevPage
        });
    }
}