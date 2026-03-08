namespace WatchHive.Controllers;

using Microsoft.AspNetCore.Mvc;
using WatchHive.Models;
using WatchHive.DTOs;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("/api/")]
public class MovieNightController : ControllerBase
{
    private readonly WatchHiveDbContext _dbContext;

    public MovieNightController(WatchHiveDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost("groups/{groupId}/movie-night")]
    public async Task<IActionResult> CreateMovieNight([FromRoute] string groupId, [FromBody] CreateMovieNightDto movieNightDto)
    {
        if (!Guid.TryParse(groupId, out Guid parsedGroupId))
        {
            return BadRequest(new CustomError { Message = "Invalid groupId provided" });
        }

        if (!Guid.TryParse(movieNightDto.CreatedById, out Guid parsedUserId))
        {
            return BadRequest(new CustomError { Message = "Invalid userId provided" });
        }

        var groupExists = await _dbContext.Groups.FindAsync(parsedGroupId);
        if (groupExists == null)
        {
            return NotFound(new CustomError{ Message = "The movie night's group does not exist" });
        }

        var userExist = await _dbContext.Users.FindAsync(parsedUserId);
        if (userExist == null)
        {
            return NotFound(new CustomError{ Message = "The user creating the movie night does not exist" });
        }

        var movieNight = new MovieNightEvent
        {
            Name = movieNightDto.Name,
            Description = movieNightDto.Description ?? "",
            CreatedById = parsedUserId,
            GroupId = parsedGroupId,
            ScheduledAt = movieNightDto.ScheduledAt
        };

        await _dbContext.MovieNightEvents.AddAsync(movieNight);

        await _dbContext.SaveChangesAsync();

        return Ok(new { Message = "Movie night event created successfully", movieNightId = movieNight.Id });
    }

    [HttpGet("groups/{groupId}/movie-nights")]
    public async Task<IActionResult> FetchMovieNights([FromRoute] string groupId,[FromQuery] string? status)
    {
        if (!Guid.TryParse(groupId, out Guid parsedGroupId))
        {
            return BadRequest(new CustomError { Message = "Invalid groupId provided" });
        }

        var groupExists = await _dbContext.Groups.FindAsync(parsedGroupId);
        if (groupExists == null)
        {
            return NotFound(new CustomError{ Message = "The movie night's group does not exist" });
        }

        // TODO: When fetching MovieNightEvent also fetch the selected movie from TMDB
        var query = _dbContext.MovieNightEvents
                    .Where(mn => mn.GroupId == parsedGroupId)
                    .Include(mn => mn.MovieSuggestions)
                        .ThenInclude(ms => ms.SuggestedBy)
                    .Include(mn => mn.MovieNightRatings)
                        .ThenInclude(mr => mr.User)
                    .AsNoTracking();

        var now = DateTimeOffset.UtcNow;
        if (!string.IsNullOrEmpty(status))
        {
            if (status == "upcoming")
            {
                query = query.Where(mn => mn.ScheduledAt > now);
            }
            else if (status == "past")
            {
                query = query.Where(mn => mn.ScheduledAt < now || mn.IsLocked);
            }
        }

        var movieEvents = await query.ToListAsync();

        // TODO: Call TMDB API to get details about the movie suggestion

        return Ok(new { movieEvents });
    }

    [HttpGet("movie-nights/{movieNightId}")]
    public async Task<IActionResult> FetchMovieNight([FromRoute] string movieNightId)
    {
        if (!Guid.TryParse(movieNightId, out Guid parsedMovieNightId))
        {
            return BadRequest(new CustomError { Message = "Invalid movieNightId provided" });
        }
        
        var MovieNight = await _dbContext.MovieNightEvents.FindAsync(parsedMovieNightId);

        if (MovieNight == null)
        {
            return NotFound(new CustomError { Message = "Movie night event not found" } );
        }

        return Ok(new { MovieNight });
    }

    [HttpPatch("movie-nights/{movieNightId}")]
    public async Task<IActionResult> UpdateMovieNight([FromRoute] string movieNightId, [FromBody] UpdateMovieNightDto updateDto)
    {
        if (!Guid.TryParse(movieNightId, out Guid parsedMovieNightId))
        {
            return BadRequest(new CustomError { Message = "Invalid movieNightId provided" });
        }
        
        var MovieNight = await _dbContext.MovieNightEvents
                            .Where(mn => mn.Id == parsedMovieNightId)
                            .FirstAsync();

        if (MovieNight == null)
        {
            return NotFound(new CustomError { Message = "Movie night event not found" } );
        }

        MovieNight.IsLocked = updateDto.IsLocked ?? MovieNight.IsLocked;
        MovieNight.ScheduledAt = updateDto.ScheduledAt ?? MovieNight.ScheduledAt;
        MovieNight.SelectedMovieTmdbId = updateDto.SelectedMovieTmdbId;

        await _dbContext.SaveChangesAsync();

        return Ok(new {Message = "Movie night updated successfully" });
    }    
}