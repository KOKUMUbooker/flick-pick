namespace WatchHive.Services;

using WatchHive.DTOs;
using WatchHive.Models;

public class VoteService
{
    public static MovieSuggestionDto ToMovieSuggestionDto(MovieSuggestion ms)
    {
        if (ms.SuggestedBy == null)
        {
            throw new Exception("Ensure navigational property suggestedBy is included in fetched vote");    
        }

        return new MovieSuggestionDto
        {
            Id = ms.Id,
            MovieId = ms.MovieId,
            MovieNightEventId = ms.MovieNightEventId,
            IsDisqualified = ms.IsDisqualified,
            // DownVoteCount = ms.DownVoteCount,
            // UpvoteCount = ms.UpvoteCount,

            SuggestedBy = new UserInfoDto
            {
                FullName = ms.SuggestedBy.FullName,
                Email = ms.SuggestedBy.Email
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
        };
    }

    public static MovieSuggestionDto ToMovieSuggestionDto(MovieSuggestion? ms, UserInfoDto suggestedBy, Movie selectedMovie)
    {
        if (ms == null)
        {
            throw new Exception("Movie suggestion not provided");    
        }

        return new MovieSuggestionDto
        {
            Id = ms.Id,
            MovieId = ms.MovieId,
            MovieNightEventId = ms.MovieNightEventId,
            IsDisqualified = ms.IsDisqualified,
            // DownVoteCount = ms.DownVoteCount,
            // UpvoteCount = ms.UpvoteCount,

            SuggestedBy = suggestedBy,

            Movie = new TMDBMovieDto
            {
                TmdbId = selectedMovie.TmdbId,
                Title = selectedMovie.Title,
                PosterPath = selectedMovie.PosterPath,
                ReleaseDate = selectedMovie.ReleaseDate.ToString(),
                Overview = selectedMovie.Overview,
                VoteAverage = selectedMovie.VoteAverage
            },
        };
    }

    // NOTE: Ensure the vote fetched includes the navigational property user before calling this method
    public static VoteDto ToVoteDto(Vote vote)
    {
        if (vote.User == null)
        {
            throw new Exception("Ensure navigational property user is included in fetched vote");    
        }

        return new VoteDto
        {
          Id = vote.Id.ToString(),
          User =  new UserInfoDto
          {
              FullName = vote.User.FullName,
              Email = vote.User.Email
          },
          MovieSuggestionId = vote.MovieSuggestionId.ToString(),
        };
    }

    public static VoteEventCountDto ToVoteEventCountDto(int upvoteCount, int downvoteCount)
    {
        return new VoteEventCountDto
        {
          DownvoteCount = downvoteCount,
          UpvoteCount = upvoteCount  
        };
    }
}