namespace FlickPickApp.Models;

public class MovieNightRating : EntityBase
{
    public Guid MovieNightEventId { get; set; }
    public MovieNightEvent MovieNightEvent { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    [Range(1,5)]
    public int Rating { get; set; }

    [MaxLength(500)]
    public string? Comment { get; set; }
}
