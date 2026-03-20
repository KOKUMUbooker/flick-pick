namespace WatchHive.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WatchHive.DTOs;
using WatchHive.Models;

[ApiController]
[Route("/api/")]
public class GroupInvitationController : ControllerBase
{
    private readonly WatchHiveDbContext _dbContext;

    public GroupInvitationController(WatchHiveDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("group/invites")]
    public async Task<IActionResult> GetGroupInvites([FromQuery] string userId)
    {
           if (!Guid.TryParse(userId, out Guid parsedUserId))
        {
            return BadRequest(new CustomError {Message = "Invalid user id provided"});
        }

        var invitations = await _dbContext.GroupInvitations
                    .Where(gi => gi.InviteeUserId == parsedUserId || gi.CreatedById == parsedUserId)
                    .AsNoTracking()
                    .Select(gi => new
                    {
                        Id = gi.Id,
                        Group = new
                        {
                            Name = gi.Group.Name,
                            Description = gi.Group.Description ?? "",
                        },
                        Invitee = new
                        {
                            FullName = gi.Invitee.FullName,
                            Email = gi.Invitee.Email
                        },
                        CreatedBy = new
                        {
                            FullName = gi.CreatedBy.FullName,
                            Email = gi.CreatedBy.Email
                        }
                    })
                    .ToListAsync();

        return Ok(new { invitations } );
    }

    [HttpPost("group/{groupId}/invite")]
    public async Task<IActionResult> CreateGroupInvitation([FromRoute] string groupId, [FromBody] CreateInvitationDto inviteDto)
    {
        if (!Guid.TryParse(groupId, out Guid parsedGroupId))
        {
            return BadRequest(new CustomError {Message = "Invalid group id provided"});
        }

        if (!Guid.TryParse(inviteDto.InviteeUserId, out Guid parsedInviteeUserId)) 
        {
            return BadRequest(new CustomError {Message = "Invalid invitee user id provided"});
        }

        if (!Guid.TryParse(inviteDto.CreatedById, out Guid parsedCreatedById)) 
        {
            return BadRequest(new CustomError {Message = "Invalid creator id provided"});
        }

        var groupExists = await _dbContext.Groups.AnyAsync(g => g.Id == parsedGroupId);
        if (!groupExists) return NotFound(new CustomError {Message="The group does not exist"} );
        
        var inviteeExists = await _dbContext.Users.AnyAsync(u => u.Id == parsedInviteeUserId);
        if (!inviteeExists) return NotFound(new CustomError {Message="The invitee does not exist"} );
        
        if (parsedInviteeUserId != parsedCreatedById)
        {
            var creatorExists = await _dbContext.Users.AnyAsync(u => u.Id == parsedCreatedById);
            if (!creatorExists) return NotFound(new CustomError {Message="The request creator does not exist"} );
        }

        // Check for an unresolved group invitation
        var pendingInvitationExists = await _dbContext.GroupInvitations
                    .AnyAsync(gi => gi.InviteeUserId == parsedInviteeUserId 
                                    && gi.GroupId == parsedGroupId
                                    && gi.Status == "pending");
        if (pendingInvitationExists)
        {
            return BadRequest(new CustomError{Message = "An earlier invitation had been sent. No new invitations allowed until the initial is resolved" });
        }

        // Check if user is already a member
        var isAlreadyAMember = await _dbContext.UserGroups
                .AnyAsync(m => m.GroupId == parsedGroupId && m.UserId == parsedInviteeUserId);
        if (isAlreadyAMember)
        {
            return BadRequest(new CustomError{Message="The user is already a member in the group"});
        }

        var invitation = new GroupInvitation
        {
            InviteeUserId = parsedInviteeUserId,
            CreatedById = parsedCreatedById,
            GroupId = parsedGroupId
        };

        await _dbContext.GroupInvitations.AddAsync(invitation);

        await _dbContext.SaveChangesAsync();

        var Message =  "Group invitation created successfully";
        if (parsedCreatedById == parsedInviteeUserId) Message = "Join request created successfully";

        return Ok(new { Message , InvitationId = invitation.Id });
    }

    [HttpGet("groups/invite")]
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
                EF.Functions.Like(g.Name, $"%{query}%")
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
                Description = g.Description
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

        if (groups.Any())
        {
            nextCursor = groups.Last().Id;
            prevCursor = groups.First().Id;
        }

        // Accurate hasNext / hasPrev
        bool hasNextPage = false;
        bool hasPrevPage = false;

        if (groups.Any())
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

    [HttpGet("users/invite")]
    public async Task<IActionResult> GetUsersToJoinGroup(
        [FromQuery] string groupId,
        [FromQuery] string query,
        [FromQuery] Guid? cursor,
        [FromQuery] int? limit,
        [FromQuery] string direction = "next" // "next" | "prev"
    )
    {
        if (!Guid.TryParse(groupId, out Guid parsedGroupId))
        {
            return BadRequest(new CustomError { Message = "Invalid group id provided" });
        }

        var groupExists = await _dbContext.Groups.AnyAsync(g => g.Id == parsedGroupId);
        if (!groupExists)
        {
            return NotFound(new CustomError { Message = "The group does not exist" });
        }

        limit ??= 10;

        // Base query (users NOT in group)
        var baseQuery = _dbContext.Users
            .Where(u => !u.UserGroups.Any(ug => ug.GroupId == parsedGroupId));

        // Search filter (safe for EF)
        if (!string.IsNullOrEmpty(query))
        {
            baseQuery = baseQuery.Where(u =>
                EF.Functions.Like(u.FullName, $"%{query}%") ||
                EF.Functions.Like(u.Email, $"%{query}%")
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