namespace FlickPickApp.Models;

public class MovieSuggestion : EntityBase
{
    [Required]
    public int TmdbId { get; set; }

    [Required]
    public Guid SuggestedById { get; set; }
    public User SuggestedBy { get; set; } = null!;

    [Required]
    public Guid MovieNightEventId { get; set; }
    public MovieNightEvent MovieNightEvent { get; set; } = null!;

    public ICollection<Vote> Votes { get; set; } = new List<Vote>();
}
