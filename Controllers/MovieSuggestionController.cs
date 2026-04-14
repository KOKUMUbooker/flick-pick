using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WatchHive.DTOs;
using WatchHive.Hubs;
using WatchHive.Models;
using WatchHive.Services;

namespace WatchHive.Controllers;

[ApiController]
[Route("/api/")]
public class MovieSuggestionController : ControllerBase
{
    private readonly WatchHiveDbContext _dbContext;
    private readonly IHubContext<MovieNightHub> _hubContext;

    public MovieSuggestionController(WatchHiveDbContext dbContext, IHubContext<MovieNightHub> hubContext)
    {
        _dbContext = dbContext;
        _hubContext = hubContext;
    }

    [HttpGet("movie-nights/{movieNightId}/suggestions")]
    public async Task<IActionResult> GetMovieNightSuggestions([FromRoute] string movieNightId,[FromQuery] string initiator)
    {
        if (!Guid.TryParse(movieNightId, out Guid parsedMovieNightId))
        {
            return BadRequest(new CustomError { Message = "Invalid movieNightId provided" });
        }

          if (!Guid.TryParse(initiator, out Guid parsedInitiatorId))
        {
            return BadRequest(new CustomError { Message = "Invalid initiator provided" });
        }
        
        var MovieNightSuggestions = await _dbContext.MovieSuggestions  
            .Where(ms => ms.MovieNightEventId == parsedMovieNightId)
            .Select(ms => new 
            {
                Id = ms.Id,
                MovieId = ms.MovieId,
                MovieNightEventId = ms.MovieNightEventId,
                IsDisqualified = ms.IsDisqualified,
                SuggestedBy = new
                {
                    fullName = ms.SuggestedBy.FullName,
                    email = ms.SuggestedBy.Email
                },
                Movie = new TMDBMovieDto
                {
                    TmdbId = ms.Movie.TmdbId,
                    Title = ms.Movie.Title,
                    PosterPath = ms.Movie.PosterPath,
                    ReleaseDate = ms.Movie.ReleaseDate.ToString(),
                    Overview = ms.Movie.Overview,
                    VoteAverage = ms.Movie.VoteAverage
                },
                UserVote = ms.Votes.Where(v => v.UserId == parsedInitiatorId).Select(v => v.VoteType).FirstOrDefault()
            })
            .AsNoTracking()
            .ToListAsync();

        return Ok(new {MovieNightSuggestions});
    }

    [HttpDelete("movie-events/{movieEventId}/suggestion/{suggestionId}")]
    public async Task<IActionResult> DeleteMovieEventSuggestion(
        [FromRoute] Guid movieEventId, 
        [FromRoute] Guid suggestionId,
        [FromBody] DeleteMovieSuggestionDto delDto
    )
    {
        var movieSuggestion = await _dbContext.MovieSuggestions.FindAsync(suggestionId);
        var Message = "Movie suggestion deleted successfully";
        if (movieSuggestion == null)
        {
            return Ok(new { Message });
        }
        
        if (!Guid.TryParse(delDto.Initiator, out Guid parsedInitiatorId))
        {
            return BadRequest(new CustomError { Message = "Invalid movieNightId provided" });
        }

        // Only allow creator of suggestion to delete it
        if (movieSuggestion.SuggestedById != parsedInitiatorId)
        {
            return BadRequest(new CustomError {Message="You are not allowed to delete this suggestion"});
        }

        await _dbContext.MovieSuggestions
            .Where(ms => ms.Id == suggestionId && ms.MovieNightEventId == movieEventId && ms.SuggestedById == parsedInitiatorId)
            .ExecuteDeleteAsync();

        await _hubContext.Clients
            .GroupExcept(movieEventId.ToString(), new[] {delDto.ConnectionId} )
            .SendAsync("suggestion", movieEventId.ToString(), new { Id =  suggestionId.ToString() }, "delete");

        return Ok(new { Message });
    }

    [HttpPost("movie-nights/{movieNightId}/suggestion")]
    public async Task<IActionResult> CreateMovieNightSuggestion([FromRoute] string movieNightId, [FromBody] CreateMovieSuggestionDto createDto)
    {
        if (!Guid.TryParse(movieNightId, out Guid parsedMovieNightId))
        {
            return BadRequest(new CustomError { Message = "Invalid movieNightId provided" });
        }

        if (!Guid.TryParse(createDto.SuggestedById, out Guid parsedUserId))
        {
            return BadRequest(new CustomError { Message = "Invalid groupId provided" });
        }

        var existingUser = await _dbContext.Users.FindAsync(parsedUserId);
        if (existingUser == null)
        {
            return NotFound(new CustomError{ Message = "The user creating the movie night does not exist" });
        }
         
        var MovieNight = await _dbContext.MovieNightEvents.FindAsync(parsedMovieNightId);
        if (MovieNight == null)
        {
            return NotFound(new CustomError { Message = "Movie night event not found" } );
        }
 
        // Check if user already made a suggestion
        var previousSuggestion = await _dbContext.MovieSuggestions
            .AnyAsync(ms => ms.SuggestedById == parsedUserId && ms.MovieNightEventId == parsedMovieNightId);
 
        if (previousSuggestion)
        {
            return BadRequest(new CustomError { Message = "Only one suggestion allowed per user in a Movie Night" });
        }
 
        // Create the movie first
        // Check if movie exists first
        var existingMovie = await _dbContext.Movies
                            .Where(m => m.TmdbId == createDto.SelectedMovie.TmdbId)
                            .AsNoTracking()
                            .FirstOrDefaultAsync();
        if (existingMovie == null)
        {
            var SelectedMovie = new Movie {
                TmdbId = createDto.SelectedMovie.TmdbId,
                Title = createDto.SelectedMovie.Title,
                PosterPath = createDto.SelectedMovie.PosterPath,
                ReleaseDate = createDto.SelectedMovie.ReleaseDate != null
                        ? DateTime.SpecifyKind(DateTime.Parse(createDto.SelectedMovie.ReleaseDate), DateTimeKind.Utc)
                        : null,
                Overview  = createDto.SelectedMovie.Overview, 
                VoteAverage  = createDto.SelectedMovie.VoteAverage,
            };
            await  _dbContext.Movies.AddAsync(SelectedMovie);
            existingMovie = SelectedMovie;
        }

        var movieSuggestion = new MovieSuggestion
        {
            MovieId = createDto.SelectedMovie.TmdbId,
            SuggestedById = parsedUserId,
            MovieNightEventId = parsedMovieNightId,
        };

        var newMovieSuggestion = await  _dbContext.MovieSuggestions.AddAsync(movieSuggestion);

        // Automatically create an upvote by the suggestor for the suggestion
        var upVote = new Vote
        {
            VoteType = VoteType.Upvote,
            MovieSuggestionId = movieSuggestion.Id,
            UserId = parsedUserId,
        };

        await _dbContext.Votes.AddAsync(upVote);

        await _dbContext.SaveChangesAsync();

        var suggestedBy = new UserInfoDto {
            Email = existingUser.Email,
            FullName = existingUser.FullName
        };

        await _hubContext.Clients
            .GroupExcept(movieSuggestion.Id.ToString(), new[] {createDto.ConnectionId} )
            .SendAsync(
                "suggestion", 
                movieSuggestion.Id.ToString(), 
                VoteService.ToMovieSuggestionDto(movieSuggestion,suggestedBy,existingMovie),
                "create"
            );

        return Ok(new { Message = "Movie suggestion created successfully"});
    }
}