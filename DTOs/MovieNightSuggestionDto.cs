using System.ComponentModel.DataAnnotations;

namespace WatchHive.DTOs;

public class CreateMovieSuggestionDto
{
    [Required]
    public int TmdbId { get; set; }

    [Required]
    public string SuggestedById { get; set; } = null!; 
}

public class MovieSuggestionDto
{
    public string Id { get; set; } = string.Empty;

    public int TmdbId { get; set; }

    public TmdbMovieDetailsDto MovieDetails { get; set; } = new();

    public UserInfoDto SuggestedBy { get; set; } = new();

    public List<VoteDto> Votes { get; set; } = new();

    public bool IsDisqualified { get; set; }
}

public class TMDBMovieDetailsDto
    {
        [JsonPropertyName("tmdbId")]
        public int TmdbId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("overview")]
        public string Overview { get; set; } = string.Empty;

        [JsonPropertyName("posterPath")]
        public string PosterPath { get; set; } = string.Empty;

        [JsonPropertyName("runtime")]
        public int Runtime { get; set; }

        [JsonPropertyName("releaseDate")]
        public string ReleaseDate { get; set; } = string.Empty;

        // Computed property for full poster URL
        [JsonIgnore]
        public string FullPosterUrl => string.IsNullOrEmpty(PosterPath) 
            ? string.Empty 
            : $"https://image.tmdb.org/t/p/w500{PosterPath}";
    }

public class UserInfoDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}