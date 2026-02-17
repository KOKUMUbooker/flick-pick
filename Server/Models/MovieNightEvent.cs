namespace FlickPickApp.Models;

public class MovieNightEvent : EntityBase
{
    [Required]
    public Guid GroupId { get; set; }
    public Group Group { get; set; } = null!;

    [Required]
    public DateTime ScheduledAt { get; set; }

    public bool IsLocked { get; set; } = false; 
    // Lock voting when event starts

    public int? SelectedMovieTmdbId { get; set; }

    public ICollection<MovieSuggestion> Suggestions { get; set; } = new List<MovieSuggestion>();
    public ICollection<MovieNightRating> Ratings { get; set; } = new List<MovieNightRating>();
    public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
}
