namespace WatchHive.Models;

using System.ComponentModel.DataAnnotations;

public class MovieNightEvent : EntityBase
{
    [Required]
    public string Name { get; set; } = null!;

    public string? Description { get; set; } = string.Empty;

    [Required]
    public Guid GroupId { get; set; }

    [Required]
    public DateTimeOffset ScheduledAt { get; set; }

    [Required]
    public Guid CreatedById {get; set;}

    // Lock voting when event starts
    public bool IsLocked { get; set; } = false;

    public int? SelectedMovieTmdbId { get; set; }

    // Navigational properties
    public Group Group { get; set; } = null!;
    public User CreatedBy {get; set; } = null!;
    public ICollection<MovieSuggestion> MovieSuggestions { get; set; } = new List<MovieSuggestion>();
    public ICollection<MovieNightRating> MovieNightRatings { get; set; } = new List<MovieNightRating>();
    public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
}
