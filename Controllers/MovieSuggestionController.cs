using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WatchHive.DTOs;
using WatchHive.Models;

namespace WatchHive.Controllers;

[ApiController]
[Route("/api/movie-nights/")]
public class MovieSuggestionController : ControllerBase
{
    private readonly WatchHiveDbContext _dbContext;

    public MovieSuggestionController(WatchHiveDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("{movieNightId}/suggestions")]
    public async Task<IActionResult> GetMovieNightSuggestions([FromRoute] string movieNightId)
    {
        if (!Guid.TryParse(movieNightId, out Guid parsedMovieNightId))
        {
            return BadRequest(new CustomError { Message = "Invalid movieNightId provided" });
        }
        
        var MovieNightSuggestions = await _dbContext.MovieSuggestions  
                            .Where(ms => ms.MovieNightEventId == parsedMovieNightId)
                            .AsNoTracking()
                            .ToListAsync();

        return Ok(new {MovieNightSuggestions});
    }

    [HttpPost("{movieNightId}/suggestion")]
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

        var MovieSuggestion = new MovieSuggestion
        {
            TmdbId = createDto.TmdbId,
            SuggestedById = parsedUserId
        };

        await  _dbContext.MovieSuggestions.AddAsync(MovieSuggestion);

        return Ok(new { Message = "Movie suggestion created successfully"});
    }
}