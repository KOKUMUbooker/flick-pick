namespace WatchHive.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WatchHive.DTOs;
using WatchHive.Hubs;
using WatchHive.Models;

[ApiController]
[Route("/api/")]
public class VoteController : ControllerBase
{
    private readonly WatchHiveDbContext _dbContext;
    private readonly IHubContext<MovieNightHub> _hubContext;

    public VoteController(WatchHiveDbContext dbContext, IHubContext<MovieNightHub> hubContext)
    {
        _dbContext = dbContext;
        _hubContext = hubContext;
    }

    [HttpGet("movie-suggestions/vote/{voteId}")]
    public async Task<IActionResult> GetVoteDetails([FromRoute] Guid voteId)
    {
        var vote = await _dbContext.Votes
                        .Where(v => v.Id == voteId)
                        .Select(v => new
                        {
                            Id = v.Id,
                            VoteType = v.VoteType,
                            MovieSuggestionId = v.MovieSuggestionId,
                            MovieNightEventId = v.MovieSuggestion.MovieNightEventId,
                            user = new
                            {
                                FullName = v.User.FullName,
                                Email = v.User.Email
                            }
                        }) 
                        .SingleOrDefaultAsync();
        
        return Ok(new {vote});
    }

    [HttpGet("movie-suggestions/{movieSuggestionId}/votes")]
    public async Task<IActionResult> GetSuggestionVotes([FromRoute] string movieSuggestionId)
    {
 
        if (!Guid.TryParse(movieSuggestionId, out Guid parsedMovieSuggestionId))
        {
            return BadRequest(new CustomError { Message = "Invalid movieSuggestionId provided" });
        }

        var votes = await _dbContext.Votes
            .Where(v => v.MovieSuggestionId == parsedMovieSuggestionId)
            .Select(v => new
            {
                Id = v.Id,
                VoteType = v.VoteType,
                MovieSuggestionId = v.MovieSuggestionId,
                MovieNightEventId = v.MovieSuggestion.MovieNightEventId,
                user = new
                {
                    FullName = v.User.FullName,
                    Email = v.User.Email
                }
            }) 
            .ToListAsync();
 
        return Ok( new { votes } );
    }

    [HttpPost("movie-suggestions/{movieSuggestionId}/vote")]
    public async Task<IActionResult> VoteForSuggestion([FromRoute] string movieSuggestionId, [FromBody] CreateVoteDto createDto)
    {
        if (!Guid.TryParse(movieSuggestionId, out Guid parsedMovieSuggestionId))
        {
            return BadRequest(new CustomError { Message = "Invalid movieSuggestionId provided" });
        }

        var movieSuggestion = await _dbContext.MovieSuggestions
            .Include(ms => ms.SuggestedBy)
            .Include(ms => ms.Movie)
            .Include(ms => ms.Votes)
            .FirstOrDefaultAsync(ms => ms.Id == parsedMovieSuggestionId);
        if (movieSuggestion == null)
        {
            return BadRequest(new CustomError { Message = "Movie suggestion to voted on does not exist" });
        }

        if (!Guid.TryParse(createDto.UserId, out Guid parsedUserId))
        {
            return BadRequest(new CustomError { Message = "Invalid groupId provided" });
        }

        // Check if Event has been locked
        bool eventLocked = await _dbContext.MovieNightEvents
                            .AnyAsync(me => me.Id == movieSuggestion.MovieNightEventId && me.IsLocked);

        if (eventLocked)
        {
            return BadRequest(new CustomError { Message = "Movie event has been locked by admin, no more votes allowed" });
        }

        var userExist = await _dbContext.Users.FindAsync(parsedUserId);
        if (userExist == null)
        {
            return NotFound(new CustomError{ Message = "The user voting for a suggestion does not exist" });
        }

        // Delete other votes made by the user for the suggestion before adding a new one
        var initialVote = await _dbContext.Votes
                .Where(v => v.UserId == parsedUserId && v.MovieSuggestionId == parsedMovieSuggestionId)
                .AsNoTracking()
                .FirstOrDefaultAsync();
       
        // If vote was found & its the same vote type, just remove it
        string movieNightId = movieSuggestion.MovieNightEventId.ToString();
        if (initialVote != null && initialVote.VoteType == createDto.VoteType)
        {
            await _dbContext.Votes 
                    .Where(v => v.UserId == parsedUserId && v.MovieSuggestionId == parsedMovieSuggestionId)
                    .ExecuteDeleteAsync();
 
            // Update movie suggestion Upvote & Downvote counts
            if (initialVote.VoteType == VoteType.Upvote && movieSuggestion.UpvoteCount > 0)
            {
                movieSuggestion.UpvoteCount--;
            }
            else if (initialVote.VoteType == VoteType.Downvote && movieSuggestion.DownVoteCount > 0)
            {
                movieSuggestion.DownVoteCount--;
            }
            
            if (createDto.VoteType == VoteType.Veto && movieSuggestion.IsDisqualified)
            {
                movieSuggestion.IsDisqualified = false;
            }

            await _hubContext.Clients
                .GroupExcept(movieNightId, new[] {createDto.ConnectionId} )
                .SendAsync("suggestion", movieNightId, ToMovieSuggestionDto(movieSuggestion,parsedUserId), "vote");

            await _dbContext.SaveChangesAsync();

            return Ok(new { Message = "Vote removed successfully" });
        }

        // If voteType is different, delete the previous one before adding a new one
        if (initialVote != null && initialVote.VoteType != createDto.VoteType)
        {
             // Update movie suggestion Upvote & Downvote counts
            if (initialVote.VoteType == VoteType.Upvote && movieSuggestion.UpvoteCount > 0)
            {
                movieSuggestion.UpvoteCount--;
            }
            if (initialVote.VoteType == VoteType.Downvote && movieSuggestion.DownVoteCount > 0)
            {
                movieSuggestion.DownVoteCount--;
            }
            
            await _dbContext.Votes 
                    .Where(v => v.UserId == parsedUserId && v.MovieSuggestionId == parsedMovieSuggestionId)
                    .ExecuteDeleteAsync();
        }

        var vote = new Vote
        {
            MovieSuggestionId = parsedMovieSuggestionId,
            UserId = parsedUserId,
            VoteType = createDto.VoteType
        };

         // Update movie suggestion Upvote & Downvote counts
        if (createDto.VoteType == VoteType.Upvote)
        {
            movieSuggestion.UpvoteCount++;
        }
        if (createDto.VoteType == VoteType.Downvote)
        {
            movieSuggestion.DownVoteCount++;
        }

        await _dbContext.Votes.AddAsync(vote);

        // If vote was a veto, disqualify the suggestion
        if (createDto.VoteType == VoteType.Veto)
        {
            movieSuggestion.IsDisqualified = true;
        } 
        else if (createDto.VoteType != VoteType.Veto && movieSuggestion.IsDisqualified)
        {
            movieSuggestion.IsDisqualified = false;
        }

        await _hubContext.Clients
            .GroupExcept(movieNightId, new[] {createDto.ConnectionId} )
            .SendAsync("suggestion", movieNightId, ToMovieSuggestionDto(movieSuggestion ,parsedUserId), "vote");
        await _dbContext.SaveChangesAsync();

        return Ok(new { Message = "Vote added successfully" });
    }

    private static Object ToMovieSuggestionDto(MovieSuggestion ms, Guid initiatorId)
    {
         return new {
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
            ms.UpvoteCount,
            ms.DownVoteCount,
            UserVote = ms.Votes.Where(v => v.UserId == initiatorId).Select(v => v.VoteType).FirstOrDefault()
        };
    }
}