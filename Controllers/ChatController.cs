namespace WatchHive.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WatchHive.DTOs;
using WatchHive.Models;

[ApiController]
[Route("/api/movie-night/")]
public class ChatController : ControllerBase
{
    private readonly WatchHiveDbContext _dbContext;

    public ChatController(WatchHiveDbContext dbContext)
    {
        _dbContext = dbContext;
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
                FullName = c.SentBy.FullName
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
}