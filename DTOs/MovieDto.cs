namespace WatchHive.DTOs;

public class MovieCreateDto
{
    public int TmdbId { get; set; }
    public string Title { get; set; } = null!;
    public string PosterPath { get; set; } = null!;
    public string? ReleaseDate { get; set; }
    public string? Overview { get; set; }
    public double? VoteAverage { get; set; }
}
