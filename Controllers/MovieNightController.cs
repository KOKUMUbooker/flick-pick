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
    public async Task<IActionResult> UpsertMovieNight(
        [FromRoute] Guid groupId, 
        [FromQuery] Guid userId,
        [FromBody] UpsertMovieNightDto movieNightDto
    )
    {
        if (!string.IsNullOrEmpty(movieNightDto.Id) && !string.IsNullOrWhiteSpace(movieNightDto.Id)) // Update logic
        {
            if (!Guid.TryParse(movieNightDto.Id, out Guid parsedEventId))
            {
                return BadRequest(new CustomError { Message = "Invalid movie event id provided" });
            }

            // Allow only group admins to update the movie event
            var isGroupAdmin = await _dbContext.UserGroups
                                    .AnyAsync(ug => ug.UserId == userId && ug.GroupId == groupId && ug.IsAdmin);
            if (!isGroupAdmin)
            {
                return BadRequest(new CustomError {Message = "You are not allowed to update this movie event"});
            }

            var movieEvent = await _dbContext.MovieNightEvents.FindAsync(parsedEventId);
            if (movieEvent == null)
            {
                return BadRequest(new CustomError{ Message = "The movie event does not exist" });
            }

            movieEvent.Name = movieNightDto.Name ?? movieEvent.Name;
            movieEvent.Description = movieNightDto.Description ?? movieEvent.Description;
            // movieEvent.ScheduledAt = movieNightDto.ScheduledAt != null ? movieNightDto.ScheduledAt : movieEvent.ScheduledAt; // TODO: Find a robust solution for this

            await _dbContext.SaveChangesAsync();

            return Ok(new {message="Movie event updated successfully",movieNightId=movieEvent.Id});
        }

        if (!Guid.TryParse(movieNightDto.CreatedById, out Guid parsedUserId))
        {
            return BadRequest(new CustomError { Message = "Invalid userId provided" });
        }

        var groupExists = await _dbContext.Groups.FindAsync(groupId);
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
            GroupId = groupId,
            ScheduledAt = movieNightDto.ScheduledAt
        };

        await _dbContext.MovieNightEvents.AddAsync(movieNight);

        await _dbContext.SaveChangesAsync();

        return Ok(new { Message = "Movie night event created successfully", movieNightId = movieNight.Id });
    }

    [HttpDelete("groups/{groupId}/movie-event/{movieEventId}")]
    public async Task<IActionResult> DeleteMovieEvent(
        [FromRoute] Guid groupId,
        [FromRoute] Guid movieEventId,
        [FromQuery] Guid userId
    )
    {
        // Allow only group admins to update the movie event
        var isGroupAdmin = await _dbContext.UserGroups
                                .AnyAsync(ug => ug.UserId == userId && ug.GroupId == groupId && ug.IsAdmin);
        if (!isGroupAdmin)
        {
            return BadRequest(new CustomError {Message = "You are not allowed to delete this movie event"});
        }

        await _dbContext.MovieNightEvents
                .Where(me => me.Id == movieEventId)
                .ExecuteDeleteAsync();

        return Ok(new {message = "Movie Event deleted successfully" });
    }

    [HttpGet("groups/{groupId}/movie-nights")]
    public async Task<IActionResult> FetchMovieNights(
        [FromRoute] string groupId,
        [FromQuery] string? status,
        [FromQuery] string initiator
    )
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

        if (!Guid.TryParse(initiator, out Guid parsedInitiator))
        {
            return BadRequest(new CustomError { Message = "Invalid initiator provided" });
        }

        var isGroupMember = await _dbContext.UserGroups
                    .AnyAsync(ug => ug.GroupId == parsedGroupId && ug.UserId == parsedInitiator);
        if (!isGroupMember)
        {
            return BadRequest(new CustomError{Message="You are not allowed to view movie events from a group you're not part of"});
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
                query = query.Where(mn => mn.ScheduledAt < now );
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

    [HttpPost("movieEvent/{movieEventId}/compute-results")]
    public async Task<IActionResult> ComputeMovieEventResults([FromRoute] string movieEventId, [FromBody] ComputeEventResultDto resultsDto)
    {
        if (!Guid.TryParse(movieEventId, out Guid parsedMovieEventId))
        {
            return BadRequest(new CustomError { Message = "Invalid movieEventId provided" });
        }

        if (!Guid.TryParse(resultsDto.Initiator, out Guid parsedInitiatorId))
        {
            return BadRequest(new CustomError { Message = "Invalid initiatorId provided" });
        }

        var movieEvent = await _dbContext.MovieNightEvents
                .Include(e => e.MovieSuggestions)
                    .ThenInclude(s => s.Votes)
                .Include(e => e.MovieSuggestions) 
                    .ThenInclude(s => s.Movie)
                .FirstOrDefaultAsync(e => e.Id == parsedMovieEventId);
        
        if (movieEvent == null)
        {
            return NotFound(new CustomError { Message = "Movie event does not exist"});
        }

        var isAdmin = await _dbContext.UserGroups
                        .AnyAsync(ug => ug.UserId == parsedInitiatorId && ug.IsAdmin);
        
        if (!isAdmin)
        {
            return BadRequest(new CustomError {Message = "Only group admins are allowed to perform this action"} );
        }

        // Lock the event
        movieEvent.IsLocked = true;

        var winner = movieEvent.MovieSuggestions
            .Select(s => new
            {
                Suggestion = new {
                    s.Id,
                    s.IsDisqualified,
                    s.MovieNightEventId,
                    s.SuggestedById,
                    s.Created,
                    Movie = new
                    {
                        s.Movie.TmdbId,
                        s.Movie.Title,
                    }
                },
                // HasVeto = s.Votes.Any(v => v.VoteType == VoteType.Veto),
                Upvotes = s.Votes.Count(v => v.VoteType == VoteType.Upvote),
                Downvotes = s.Votes.Count(v => v.VoteType == VoteType.Downvote)
            })
            .Where(s => !s.Suggestion.IsDisqualified)
            .Select(x => new
            {
                x.Suggestion,
                Score = x.Upvotes - x.Downvotes,
                x.Upvotes,
                x.Downvotes
            })
            .OrderByDescending(x => x.Score)       // primary
            .ThenByDescending(x => x.Upvotes)      // tie-breaker 1
            .ThenBy(x => x.Downvotes)              // tie-breaker 2
            .ThenBy(x => x.Suggestion.Created)  // tie-breaker 3 (fairness)
            .FirstOrDefault();

        if (winner == null)
        {
            return BadRequest(new CustomError { Message = "All suggestions got vetoed" });
        }

        movieEvent.SelectedMovieId = winner.Suggestion.Movie.TmdbId;
        await _dbContext.SaveChangesAsync();
        
        return Ok( new { Message = $"The movie ${winner.Suggestion.Movie.Title} has been selected for the event {movieEvent.Name}" } );
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