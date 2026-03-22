namespace WatchHive.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WatchHive.DTOs;
using WatchHive.Models;

[ApiController]
[Route("/api/")]
public class VoteController : ControllerBase
{
    private readonly WatchHiveDbContext _dbContext;

    public VoteController(WatchHiveDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("movie-suggestions/{movieSuggestionId}/vote")]
    public async Task<IActionResult> GetSuggestionVotes([FromRoute] string movieSuggestionId)
    {
        // TODO: Implement this
        return Ok();
    }

    [HttpPost("movie-suggestions/{movieSuggestionId}/vote")]
    public async Task<IActionResult> VoteForSuggestion([FromRoute] string movieSuggestionId, [FromBody] CreateVoteDto createDto)
    {
        if (!Guid.TryParse(movieSuggestionId, out Guid parsedMovieSuggestionId))
        {
            return BadRequest(new CustomError { Message = "Invalid movieSuggestionId provided" });
        }

        var movieSuggestionExists = await _dbContext.MovieSuggestions.FindAsync(parsedMovieSuggestionId);
        if (movieSuggestionExists == null)
        {
            return BadRequest(new CustomError { Message = "Movie suggestion to voted on does not exist" });
        }

        if (!Guid.TryParse(createDto.UserId, out Guid parsedUserId))
        {
            return BadRequest(new CustomError { Message = "Invalid groupId provided" });
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
        if (initialVote != null && initialVote.VoteType == createDto.VoteType)
        {
            await _dbContext.Votes 
                    .Where(v => v.UserId == parsedUserId && v.MovieSuggestionId == parsedMovieSuggestionId)
                    .ExecuteDeleteAsync();
            
            if (createDto.VoteType == VoteType.Veto && movieSuggestionExists.IsDisqualified)
            {
                movieSuggestionExists.IsDisqualified = false;
                await _dbContext.SaveChangesAsync();
            }

            return Ok(new { Message = "Vote removed successfully" });
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
            movieSuggestionExists.IsDisqualified = true;
        } else if (createDto.VoteType != VoteType.Veto && movieSuggestionExists.IsDisqualified)
        {
            movieSuggestionExists.IsDisqualified = false;
        }

        await _dbContext.SaveChangesAsync();

        return Ok(new { Message = "Vote added successfully" });
    }
}