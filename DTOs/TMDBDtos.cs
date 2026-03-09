namespace WatchHive.DTOs;
  
public class TMDBMovieDto
{
    public int TmdbId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string PosterPath {get; set;} = string.Empty;
    public string? Overview { get; set; } = string.Empty;
    public string? ReleaseDate { get; set; }
    public double? VoteAverage {get; set; }
}
