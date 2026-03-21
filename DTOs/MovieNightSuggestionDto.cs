using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WatchHive.Models;

namespace WatchHive.DTOs;

public class CreateMovieSuggestionDto
{
    [Required]
    public MovieCreateDto SelectedMovie { get; set; } = null!;

    [Required]
    public string SuggestedById { get; set; } = null!; 

}

public class DeleteMovieSuggestionDto
{
    [Required]
    public string Initiator { get; set; } = null!; 
}

public class MovieSuggestionDto
{
    public string Id { get; set; } = string.Empty;

    public int TmdbId { get; set; }

    public TMDBMovieDetailsDto MovieDetails { get; set; } = new();

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

/// <summary>
/// Vote DTO with proper enum handling
/// </summary>
public class VoteDto
{
    [JsonPropertyName("userId")]
    public string UserId { get; set; } = string.Empty;

    [JsonPropertyName("movieSuggestionId")]
    public string MovieSuggestionId { get; set; } = string.Empty;

    [JsonPropertyName("voteType")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public VoteType VoteType { get; set; }

    [JsonPropertyName("user")]
    public UserInfoDto User { get; set; } = new();

    [JsonPropertyName("votedAt")] // Optional: track when vote was cast
    public DateTime? VotedAt { get; set; }
}