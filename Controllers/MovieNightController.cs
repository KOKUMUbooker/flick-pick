namespace WatchHive.Controllers;

using Microsoft.AspNetCore.Mvc;
using WatchHive.Models;
using WatchHive.DTOs;
using Microsoft.EntityFrameworkCore;
using WatchHive.Hubs;
using Microsoft.AspNetCore.SignalR;
using WatchHive.Services;
using Microsoft.AspNetCore.Authorization;

[Authorize]
[ApiController]
[Route("/api/")]
public class MovieNightController : ControllerBase
{
    private readonly WatchHiveDbContext _dbContext;
      private readonly IHubContext<MovieNightHub> _hubContext;

    public MovieNightController(WatchHiveDbContext dbContext, IHubContext<MovieNightHub> hubContext)
    {
        _dbContext = dbContext;
        _hubContext = hubContext;
    }

    [HttpPost("groups/{groupId}/movie-night")]
    public async Task<IActionResult> CreateMovieNightEvent(
        [FromRoute] Guid groupId, 
        [FromQuery] Guid userId,
        [FromBody] CreateMovieNightDto movieNightDto
    )
    {
        if (!Guid.TryParse(movieNightDto.CreatedById, out Guid parsedUserId))
        {
            return BadRequest(new CustomError { Message = "Invalid userId provided" });
        }

        var groupExists = await _dbContext.Groups.FindAsync(groupId);
        if (groupExists == null)
        {
            return NotFound(new CustomError{ Message = "The movie night's group does not exist, please reload the page" });
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

        return Ok(new { Message = "Movie night event created successfully" });
    }

    [HttpPatch("groups/{groupId}/movie-night/edit")]
    public async Task<IActionResult> UpdateMovieNight(
        [FromRoute] Guid groupId, 
        [FromQuery] Guid userId,
        [FromBody] UpdateMovieNightDto movieNightDto
    )
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

        var movieEvent = await _dbContext.MovieNightEvents
            .Where(me => me.Id == parsedEventId)
            .Include(me => me.SelectedMovie)
            .Include(me => me.MovieNightRatings)
            .FirstOrDefaultAsync();
        if (movieEvent == null)
        {
            return BadRequest(new CustomError{ Message = "The movie event does not exist" });
        }

        movieEvent.Name = movieNightDto.Name ?? movieEvent.Name;
        movieEvent.Description = movieNightDto.Description ?? movieEvent.Description;
        movieEvent.ScheduledAt = movieNightDto.ScheduledAt != null? movieNightDto.ScheduledAt.Value : movieEvent.ScheduledAt;

        var movieEventDto = MovieNightEventService.ToMovieNightEventDto(movieEvent);
        if (movieNightDto.ConnectionId != null)
        {
            await _hubContext.Clients
                .GroupExcept(movieEvent.Id.ToString(), new[] {movieNightDto.ConnectionId} )
                .SendAsync("movieEvent", groupId, movieEventDto, "edit", userId);
        }
        await _dbContext.SaveChangesAsync();

        return Ok(new { message="Movie event updated successfully", movieEvent = movieEventDto  });
    }

    [HttpDelete("groups/{groupId}/movie-event/{movieEventId}")]
    public async Task<IActionResult> DeleteMovieEvent(
        [FromRoute] Guid groupId,
        [FromRoute] Guid movieEventId,
        [FromBody] DeleteMovieEventEventDto DelDto
    )
    {
        if (!Guid.TryParse(DelDto.Initiator, out Guid parsedInitiatorId))
        {
            return BadRequest(new CustomError { Message = "Invalid userId provided" });
        }

        // Allow only group admins to update the movie event
        var isGroupAdmin = await _dbContext.UserGroups
            .AnyAsync(ug => ug.UserId == parsedInitiatorId && ug.GroupId == groupId && ug.IsAdmin);
        if (!isGroupAdmin)
        {
            return BadRequest(new CustomError {Message = "You are not allowed to delete this movie event"});
        }

        await _dbContext.MovieNightEvents
                .Where(me => me.Id == movieEventId)
                .ExecuteDeleteAsync();

        await _hubContext.Clients
                .GroupExcept(movieEventId.ToString(), new[] {DelDto.ConnectionId} )
                .SendAsync("movieEvent", groupId, new { Id = movieEventId }, "delete", DelDto.Initiator);

        return Ok(new {message = "Movie Event deleted successfully", movieEvent = new { Id = movieEventId } });
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

     [HttpPost("movieEvent/{movieEventId}/undo-selection")]
    public async Task<IActionResult> UndoMovieSelection([FromRoute] string movieEventId, [FromBody] ComputeEventResultDto undoDto)
    {
        if (!Guid.TryParse(movieEventId, out Guid parsedMovieEventId))
        {
            return BadRequest(new CustomError { Message = "Invalid movieEventId provided" });
        }

        if (!Guid.TryParse(undoDto.Initiator, out Guid parsedInitiatorId))
        {
            return BadRequest(new CustomError { Message = "Invalid initiatorId provided" });
        }

       var movieEvent = await _dbContext.MovieNightEvents
            .Where(me => me.Id == parsedMovieEventId)
            .Include(me => me.SelectedMovie)
            .Include(me => me.MovieNightRatings)
            .FirstOrDefaultAsync();

        if (movieEvent == null)
        {
            return BadRequest(new CustomError{ Message = "The movie event does not exist" });
        }

        var isAdmin = await _dbContext.UserGroups
            .AnyAsync(ug => ug.UserId == parsedInitiatorId && ug.IsAdmin && ug.GroupId == movieEvent.GroupId);
        
        if (!isAdmin)
        {
            return BadRequest(new CustomError {Message = "Only group admins are allowed to perform this action"} );
        }

        movieEvent.SelectedMovieId = null;
        movieEvent.IsLocked = false;

        await _dbContext.SaveChangesAsync();
         var movieEventDto = MovieNightEventService.ToMovieNightEventDto(movieEvent);
        if (undoDto.ConnectionId != null)
        {
            await _hubContext.Clients
                .GroupExcept(movieEvent.Id.ToString(), new[] {undoDto.ConnectionId} )
                .SendAsync("movieEvent", movieEvent.GroupId, movieEventDto, "edit", undoDto.Initiator);
        }

        return Ok(new { message="Movie event updated successfully", movieEvent = movieEventDto  });
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
            .Include(me => me.SelectedMovie)
            .Include(me => me.MovieNightRatings)
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
            .AnyAsync(ug => ug.UserId == parsedInitiatorId && ug.IsAdmin && ug.GroupId == movieEvent.GroupId);
        
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
                        VoteAverage = s.Movie?.VoteAverage ?? 0
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
                x.Downvotes,
                MovieTmdbRating = x.Suggestion.Movie.VoteAverage,
            })
            .OrderByDescending(x => x.Score)                // primary
            .ThenByDescending(x => x.Upvotes)               // tie-breaker 1 (Highest upvotes - most popular)
            .ThenBy(x => x.Downvotes)                       // tie-breaker 2 (Least upvotes - least disliked)
            .ThenByDescending(x => x.MovieTmdbRating)       // tie-breaker 3 (Better movie based on TMDB rating)
            .ThenBy(x => x.Suggestion.Created)              // tie-breaker 4 (fairness - Suggestion that got created first)
            .FirstOrDefault();

        if (winner == null)
        {
            return BadRequest(new CustomError { Message = "All suggestions got vetoed" });
        }

        movieEvent.SelectedMovieId = winner.Suggestion.Movie.TmdbId;
        await _dbContext.SaveChangesAsync();

        var movieEventDto =  MovieNightEventService.ToMovieNightEventDto(movieEvent);
        await _hubContext.Clients
            .GroupExcept(movieEvent.Id.ToString(), new[] {resultsDto.ConnectionId} )
            .SendAsync("movieEvent", movieEvent.GroupId, movieEventDto, "edit", resultsDto);

        return Ok( new { 
            Message = $"The movie {winner.Suggestion.Movie.Title} has been selected for the event {movieEvent.Name}",
            movieEvent = movieEventDto 
        });
    }
}