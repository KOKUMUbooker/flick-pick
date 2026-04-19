namespace WatchHive.Controllers;

using Microsoft.AspNetCore.Mvc;
using WatchHive.Models;
using WatchHive.DTOs;
using Microsoft.EntityFrameworkCore;
using WatchHive.Hubs;
using Microsoft.AspNetCore.SignalR;
 
[ApiController]
[Route("/api")]
public class MovieRatingController : ControllerBase
{
    private readonly WatchHiveDbContext _dbContext;
    private readonly IHubContext<MovieNightHub> _hubContext;

    public MovieRatingController(WatchHiveDbContext dbContext, IHubContext<MovieNightHub> hubContext)
    {
        _dbContext = dbContext;
        _hubContext = hubContext;
    }

    [HttpPost("movie-event/{movieEventId}")]
    public async Task<IActionResult> RateMovieEvent(
        [FromRoute] string movieEventId, 
        [FromQuery] string userId,
        [FromBody] CreateRatingDto createDto
    )
    {
        if (!Guid.TryParse(movieEventId, out Guid parsedEventId))
        {
            return BadRequest(new CustomError { Message = "Invalid movie event id provided" });
        }

        if (!Guid.TryParse(userId, out Guid parsedUserId))
        {
            return BadRequest(new CustomError { Message = "Invalid user id provided" });
        }


        var movieEvent = await _dbContext.MovieNightEvents
            .Where(me => me.Id == parsedEventId)
            .FirstOrDefaultAsync();
        if (movieEvent == null)
        {
            return BadRequest(new CustomError{ Message = "The movie event does not exist" });
        }

        var isGroupMember = await _dbContext.UserGroups
                .AnyAsync(ug => ug.UserId == parsedUserId && ug.GroupId == movieEvent.GroupId);
        if (!isGroupMember)
        {
            return BadRequest(new CustomError {Message = "You are not allowed rate this movie event"});
        }

        var newRating = new MovieNightRating
        {
            Comment = createDto.Comment,
            Rating = createDto.Rating,
            UserId = parsedUserId,
        };

        await _dbContext.AddAsync(newRating);
        await _dbContext.SaveChangesAsync();

        var result = await _dbContext.MovieNightRatings
            .Where(r => r.MovieNightEventId == parsedEventId)
            .GroupBy(r => r.MovieNightEventId)
            .Select(g => new
            {
                AverageRating = g.Average(r => (double)r.Rating),
                TotalRatings = g.Count()
            })
            .FirstOrDefaultAsync();

        var partialMovieEvent = new
        {
            Id = parsedEventId,
            AverageRating = Math.Round(result?.AverageRating ?? 0, 1),
            TotalRatings = result != null ? result.TotalRatings : 0,
        };

        await _hubContext.Clients
            .GroupExcept(movieEventId.ToString(), new[] {createDto.ConnectionId} )
            .SendAsync("movieEvent", movieEvent.GroupId, partialMovieEvent, "rate", parsedEventId);

        return Ok(new {message = "Movie rating added successfully", movieEvent = partialMovieEvent });
    }
}