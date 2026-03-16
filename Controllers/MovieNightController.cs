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

        var now = DateTimeOffset.UtcNow;

        var query = _dbContext.MovieNightEvents
            .Where(mn => mn.GroupId == parsedGroupId)
            .AsNoTracking();

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

        var movieEvents = await query
            .Select(mn => new MovieNightEventDto
            {
                Id = mn.Id,
                Name = mn.Name,
                Description = mn.Description,
                ScheduledAt = mn.ScheduledAt,
                IsLocked = mn.IsLocked,

                SelectedMovie = mn.SelectedMovie == null ? null : new TMDBMovieDto
                {
                    TmdbId = mn.SelectedMovie.TmdbId,
                    Title = mn.SelectedMovie.Title,
                    PosterPath = mn.SelectedMovie.PosterPath,
                    ReleaseDate = mn.SelectedMovie.ReleaseDate.ToString(),
                    Overview = mn.SelectedMovie.Overview,
                    VoteAverage = mn.SelectedMovie.VoteAverage
                },

                AverageRating = status == "past"
                    ? mn.MovieNightRatings.Average(r => (double?)r.Rating)
                    : null,

                TotalRatings = status == "past"
                    ? mn.MovieNightRatings.Count
                    : 0
            })
            .ToListAsync();

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
        MovieNight.SelectedMovieId = updateDto.SelectedMovieTmdbId;

        await _dbContext.SaveChangesAsync();

        return Ok(new {Message = "Movie night updated successfully" });
    }    
}