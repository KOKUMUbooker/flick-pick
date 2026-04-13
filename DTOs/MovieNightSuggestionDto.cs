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

    [Required]
    public string ConnectionId { get; set; } = null!; 
}

public class DeleteMovieSuggestionDto
{
    [Required]
    public string Initiator { get; set; } = null!; 

    [Required]
    public string ConnectionId { get; set; } = null!; 
}

public class MovieSuggestionDto
{
    public Guid Id { get; set; }
    public int MovieId { get; set; }
    public Guid MovieNightEventId { get; set; }
    public bool IsDisqualified { get; set; }

    public UserInfoDto SuggestedBy { get; set; } = null!;
    public TMDBMovieDto Movie { get; set; } = null!;

    public int UpvoteCount { get; set; }
    public int DownVoteCount { get; set; }

    public VoteType? UserVote { get; set; }
}
// public class SuggestedByDto
// {
//     public string FullName { get; set; } = null!;
//     public string Email { get; set; } = null!;
// }

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

public class VoteDto
{
    public string Id { get; set; } = string.Empty;
    public required UserInfoDto User { get; set; }
    public VoteType VoteType { get; set; }
    public string MovieSuggestionId { get; set; } = string.Empty;
}

public class VoteCountDto
{
    public int UpvoteCount { get; set; }
    public int DownvoteCount { get; set; }
}
