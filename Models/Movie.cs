using System.ComponentModel.DataAnnotations;

namespace WatchHive.Models;

public class Movie
{
    [Key]
    public int TmdbId { get; set; }

    public string Title { get; set; } = null!;

    public string PosterPath { get; set; } = null!;

    public DateTime? ReleaseDate { get; set; }

    public string? Overview { get; set; }

    public double? VoteAverage { get; set; }

    // Navigational properties
    public ICollection<MovieSuggestion> MovieSuggestions { get; set; } = new List<MovieSuggestion>();
    public ICollection<MovieNightEvent> MovieNightEvents { get; set; } = new List<MovieNightEvent>();
}