using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WatchHive.DTOs;
using WatchHive.Models;

namespace WatchHive.Controllers;

[ApiController]
[Route("/api/")]
public class MovieSuggestionController : ControllerBase
{
    private readonly WatchHiveDbContext _dbContext;

    public MovieSuggestionController(WatchHiveDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("movie-nights/{movieNightId}/suggestions")]
    public async Task<IActionResult> GetMovieNightSuggestions([FromRoute] string movieNightId)
    {
        if (!Guid.TryParse(movieNightId, out Guid parsedMovieNightId))
        {
            return BadRequest(new CustomError { Message = "Invalid movieNightId provided" });
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
                                } 
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

        var rows = await _dbContext.MovieSuggestions
            .Where(ms => ms.Id == suggestionId && ms.MovieNightEventId == movieEventId && ms.SuggestedById == parsedInitiatorId)
            .ExecuteDeleteAsync();

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

        var userExist = await _dbContext.Users.FindAsync(parsedUserId);
        if (userExist == null)
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

        var MovieSuggestion = new MovieSuggestion
        {
            MovieId = createDto.SelectedMovie.TmdbId,
            SuggestedById = parsedUserId,
            MovieNightEventId = parsedMovieNightId
        };

        await  _dbContext.MovieSuggestions.AddAsync(MovieSuggestion);

        // Automatically create an upvote by the suggestor for the suggestion
        var upVote = new Vote
        {
            VoteType = VoteType.Upvote,
            MovieSuggestionId = MovieSuggestion.Id,
            UserId = parsedUserId,
        };

        await _dbContext.Votes.AddAsync(upVote);

        await _dbContext.SaveChangesAsync();

        return Ok(new { Message = "Movie suggestion created successfully"});
    }
}