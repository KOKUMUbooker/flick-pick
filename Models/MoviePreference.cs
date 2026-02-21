namespace FlickPickApp.Models;
using System.ComponentModel.DataAnnotations;

public class MoviePreference : EntityBase
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(50)]
    public required string Genre { get; set; }

    // Navigational properties
    public User User { get; set; } = null!;
}
