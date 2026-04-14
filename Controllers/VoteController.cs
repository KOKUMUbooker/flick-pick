namespace WatchHive.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WatchHive.DTOs;
using WatchHive.Hubs;
using WatchHive.Models;
using WatchHive.Services;

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

    [HttpGet("movie-suggestions/{movieSuggestionId}/voteCount")]
    public async Task<IActionResult> GetSuggestionVoteCount([FromRoute] string movieSuggestionId, [FromQuery] string initiator)
    {
        if (!Guid.TryParse(movieSuggestionId, out Guid parsedMovieSuggestionId))
        {
            return BadRequest(new CustomError { Message = "Invalid movieSuggestionId provided" });
        }

        if (!Guid.TryParse(initiator, out Guid parsedInitiatorId))
        {
            return BadRequest(new CustomError { Message = "Invalid initiator provided" });
        }

        var voteData = await GetVoteCountData(parsedMovieSuggestionId,parsedInitiatorId);
 
        return Ok( new { voteData } );
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
 
            if (createDto.VoteType == VoteType.Veto && movieSuggestion.IsDisqualified)
            {
                movieSuggestion.IsDisqualified = false;
            }
            
            await _dbContext.SaveChangesAsync();
            // var voteCountDetails = VoteService.ToVoteEventCountDto(movieSuggestion.UpvoteCount, movieSuggestion.DownVoteCount);
            var voteCountDetails = await GetVoteCountData(parsedMovieSuggestionId,parsedUserId);
            var upToDateMovieSuggestion = VoteService.ToMovieSuggestionDto(movieSuggestion);
            if (createDto.VoteType == VoteType.Veto)
            {
                await _hubContext.Clients
                    .GroupExcept(movieNightId, new[] {createDto.ConnectionId} )
                    .SendAsync("suggestion", movieNightId, upToDateMovieSuggestion, "edit");
            } else 
            {
                await _hubContext.Clients
                    .GroupExcept(movieNightId, new[] {createDto.ConnectionId} )
                    .SendAsync("vote", movieSuggestionId,  voteCountDetails );
            }

            return Ok(new { 
                Message = "Vote removed successfully" ,
                Data = new { voteCountData = voteCountDetails, movieSuggestion = upToDateMovieSuggestion }
            });
        }

        // If voteType is different, delete the previous one before adding a new one
        if (initialVote != null && initialVote.VoteType != createDto.VoteType)
        {
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

        await _dbContext.SaveChangesAsync();
        // var voteCountData = VoteService.ToVoteEventCountDto(movieSuggestion.UpvoteCount, movieSuggestion.DownVoteCount);
        var voteCountData = await GetVoteCountData(parsedMovieSuggestionId,parsedUserId);
        var updatedMovieSuggestion =  VoteService.ToMovieSuggestionDto(movieSuggestion);
        if (createDto.VoteType == VoteType.Veto)
        {
            await _hubContext.Clients
                .GroupExcept(movieNightId, new[] { createDto.ConnectionId } )
                .SendAsync("suggestion", movieNightId, updatedMovieSuggestion, "edit");
        } else 
        {
            await _hubContext.Clients
                .GroupExcept(movieNightId, new[] {createDto.ConnectionId} )
                .SendAsync("vote", movieSuggestionId, voteCountData );
        }

        return Ok(new {
            Message = "Vote added successfully", 
            Data = new { voteCountData , movieSuggestion = updatedMovieSuggestion }
        });
    }

    private async Task<VoteCountDto?> GetVoteCountData(Guid movieSuggestionId, Guid initiatorId)
    {
         var voteData = await _dbContext.Votes
            .Where(v => v.MovieSuggestionId == movieSuggestionId)
            .GroupBy(v => v.MovieSuggestionId)
            .Select(g => new VoteCountDto
            {
                UpvoteCount = g.Count(v => v.VoteType == VoteType.Upvote),
                DownvoteCount = g.Count(v => v.VoteType == VoteType.Downvote),
                UserVote = g
                    .Where(v => v.UserId == initiatorId)
                    .Select(v => (VoteType?)v.VoteType)
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync();

            return voteData;
    }
}