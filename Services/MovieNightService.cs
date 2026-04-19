namespace WatchHive.Services;

using WatchHive.DTOs;
using WatchHive.Models;

public class MovieNightEventService
{
    public static MovieNightEventDto ToMovieNightEventDto(MovieNightEvent me)
    {
        var movieNightEventDto =  new MovieNightEventDto
        {
            Id = me.Id,
            Name = me.Name,
            IsLocked = me.IsLocked,
            ScheduledAt = me.ScheduledAt,
            Description = me.Description,
            TotalRatings = me.MovieNightRatings.Count,
            AverageRating = me.MovieNightRatings
                .Select(r => (double?)r.Rating)
                .Average() ?? 0,
        };

        if (me.SelectedMovie != null)
        {
          movieNightEventDto.SelectedMovie =   new TMDBMovieDto
            {
                TmdbId = me.SelectedMovie.TmdbId,
                Title = me.SelectedMovie.Title,
                PosterPath = me.SelectedMovie.PosterPath,
                ReleaseDate = me.SelectedMovie.ReleaseDate.ToString(),
                Overview = me.SelectedMovie.Overview,
                VoteAverage = me.SelectedMovie.VoteAverage
            };
        }

        return movieNightEventDto;
    }

}