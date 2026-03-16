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
                                MovieId = ms.MovieId,
                                SuggestedById = ms.SuggestedById,
                                MovieNightEventId = ms.MovieNightEventId,
                                IsDisqualified = ms.IsDisqualified,
                                Movie = new MovieDto
                                {
                                    TmdbId = ms.Movie.TmdbId.ToString(),
                                    Title = ms.Movie.Title,
                                    PosterPath = ms.Movie.PosterPath,
                                    ReleaseDate = ms.Movie.ReleaseDate,
                                    Overview = ms.Movie.Overview,
                                    VoteAverage = ms.Movie.VoteAverage
                                },
                                Votes = ms.Votes.Select(v => new 
                                {
                                    VoteType = v.VoteType,
                                    UserId = new {
                                        fullName = v.FullName,
                                        email = v.Email
                                    }
                                }).ToList()
                            })
                            .AsNoTracking()
                            .ToListAsync();

        return Ok(new {MovieNightSuggestions});
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

        Console.WriteLine("===================== 1 ============================");
        var userExist = await _dbContext.Users.FindAsync(parsedUserId);
        if (userExist == null)
        {
            return NotFound(new CustomError{ Message = "The user creating the movie night does not exist" });
        }
        Console.WriteLine("===================== 2 ============================");
        
        var MovieNight = await _dbContext.MovieNightEvents.FindAsync(parsedMovieNightId);
        if (MovieNight == null)
        {
            return NotFound(new CustomError { Message = "Movie night event not found" } );
        }
        Console.WriteLine("===================== 3 ============================");

        // Check if user already made a suggestion
        var previousSuggestion = await _dbContext.MovieSuggestions
            .AnyAsync(ms => ms.SuggestedById == parsedUserId && ms.MovieNightEventId == parsedMovieNightId);
        Console.WriteLine("===================== 4 ============================");

        if (previousSuggestion)
        {
            return BadRequest(new CustomError { Message = "Only one suggestion allowed per user in a Movie Night" });
        }
        Console.WriteLine("===================== 5 ============================ ");

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

        await _dbContext.SaveChangesAsync();

        return Ok(new { Message = "Movie suggestion created successfully"});
    }
}