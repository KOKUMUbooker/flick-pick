namespace WatchHive.Services;

using WatchHive.DTOs;
using WatchHive.Models;

public class VoteService
{
    public static MovieSuggestionDto ToMovieSuggestionDto(MovieSuggestion ms, VoteType? userVote)
    {
        return new MovieSuggestionDto
        {
            Id = ms.Id,
            MovieId = ms.MovieId,
            MovieNightEventId = ms.MovieNightEventId,
            IsDisqualified = ms.IsDisqualified,

            SuggestedBy = new SuggestedByDto
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

            UpvoteCount = ms.UpvoteCount,
            DownVoteCount = ms.DownVoteCount,

            UserVote = userVote
        };
    }
}