namespace WatchHive.DTOs;

public class TMDBMovieDto
{
    public int TmdbId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
    public string PosterPath {get; set;} = string.Empty;
    public int Runtime {get; set; }
    public string ReleaseDate { get; set; } = string.Empty;
}