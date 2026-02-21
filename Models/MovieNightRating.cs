namespace FlickPickApp.Models;
using System.ComponentModel.DataAnnotations;

public class MovieNightRating : EntityBase
{
    [Required]
    public Guid MovieNightEventId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Range(1,5)]
    public int Rating { get; set; }

    [MaxLength(500)]
    public string? Comment { get; set; }


    // Navigational properties
    public User User { get; set; } = null!;
    public MovieNightEvent MovieNightEvent { get; set; } = null!;
}
