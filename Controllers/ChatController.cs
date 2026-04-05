namespace WatchHive.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using WatchHive.DTOs;
using WatchHive.Models;
using WatchHive.Hubs;

[ApiController]
[Route("/api/movie-night/")]
public class ChatController : ControllerBase
{
    private readonly WatchHiveDbContext _dbContext;
    private readonly IHubContext<MovieNightHub> _hubContext;

    public ChatController(WatchHiveDbContext dbContext, IHubContext<MovieNightHub> hubContext)
    {
        _dbContext = dbContext;
        _hubContext = hubContext;
    }

    // Implement cursor based pagination where contents are fetched from newest to less new
    // ie on UI fetch like 20 chats then when user scrolls to top another request for the 
    // current cursor to earlier 20 chats are fetched & sent to UI 
    // Server should also specify whether we've reached the end or not
    [HttpGet("{movieNightId}/chat")]
    public async Task<IActionResult> GetMovieNightChats(
        [FromRoute] string movieNightId,
        [FromQuery] DateTime? before,
        [FromQuery] int limit = 20)
    {
        if (!Guid.TryParse(movieNightId, out Guid parsedMovieNightId))
        {
            return BadRequest(new CustomError { Message = "Invalid movieNightId provided" });
        }

        var movieNight = await _dbContext.MovieNightEvents
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == parsedMovieNightId);

        if (movieNight == null)
        {
            return NotFound(new CustomError { Message = "Movie night event not found" });
        }

        // Safety cap
        limit = Math.Clamp(limit, 1, 50);

        var query = _dbContext.ChatMessages
            .AsNoTracking()
            .Where(c => c.MovieNightEventId == parsedMovieNightId);

        // Apply cursor filter (fetch older messages)
        if (before.HasValue)
        {
            query = query.Where(c => c.SentAt < before.Value);
        }

        var messages = await query
            .OrderByDescending(c => c.SentAt)
            .Take(limit + 1) // take one extra to determine if more exist
            .Select(c => new
            {
                c.Id,
                c.UserId,
                c.Message,
                c.SentAt,
                user = new
                {
                    c.SentBy.FullName,
                    c.SentBy.Email
                }
            })
            .ToListAsync();

        var hasMore = messages.Count > limit;

        if (hasMore)
        {
            messages = messages.Take(limit).ToList();
        }

        // Reverse so UI receives oldest → newest
        messages.Reverse();

        var nextCursor = messages.FirstOrDefault()?.SentAt;

        return Ok(new
        {
            messages,
            nextCursor,
            hasMore
        });
    }

    [HttpPost("{movieNightId}/chat")]
    public async Task<IActionResult> CreateMovieChat([FromRoute] string movieNightId,[FromBody] CreateChatMsgDto createDto)
    {
        if (!Guid.TryParse(movieNightId, out Guid parsedMovieNightId))
        {
            return BadRequest(new CustomError { Message = "Invalid movieNightId provided" });
        }

        if (!Guid.TryParse(createDto.UserId, out Guid parsedUserId))
        {
            return BadRequest(new CustomError { Message = "Invalid user Id provided" });
        }

        var movieEventExist = await _dbContext.MovieNightEvents
                                .AnyAsync(me => me.Id == parsedMovieNightId);
        if (!movieEventExist)
        {
            return NotFound(new CustomError {Message = "Movie event does not exist"});
        }

        var groupMemberDetails = await _dbContext.UserGroups
                                .Where(ug => ug.UserId == parsedUserId)
                                .Select(ug => new
                                {
                                    email = ug.User.Email,
                                    fullName = ug.User.FullName,
                                })
                                .AsNoTracking()
                                .FirstOrDefaultAsync();

        if (groupMemberDetails == null)
        {
            return BadRequest(new CustomError {Message = "Not allowed. You're not a memeber of this group"});
        }

        var newMsg = new ChatMessage
        {
            UserId  = parsedUserId,
            Message = createDto.Message,
            MovieNightEventId = parsedMovieNightId,
            SentAt = createDto.SentAt,
        };

        await _dbContext.ChatMessages.AddAsync(newMsg);
        await _dbContext.SaveChangesAsync();

        var msgToSend = new
        {
            id = newMsg.Id,
            userId  = parsedUserId,
            message = createDto.Message,
            movieNightEventId = parsedMovieNightId,
            sentAt = createDto.SentAt,
            user = groupMemberDetails
        };

        await _hubContext.Clients
            .GroupExcept(movieNightId, new[] {createDto.ConnectionId} )
            .SendAsync("msg", movieNightId, msgToSend);

        return Ok(new {Message = "Message created successfully", msgId  = newMsg.Id.ToString() });
    }
}