namespace WatchHive.Models;
using System.ComponentModel.DataAnnotations;

public class MovieSuggestion : EntityBase
{
    [Required]
    public int TmdbId { get; set; }

    [Required]
    public Guid SuggestedById { get; set; }

    [Required]
    public Guid MovieNightEventId { get; set; }

    public bool IsDisqualified { get; set; } = false;

    // Navigation properties
    public MovieNightEvent MovieNightEvent { get; set; } = null!;
    public ICollection<Vote> Votes { get; set; } = new List<Vote>();
    public User SuggestedBy { get; set; } = null!;
}
