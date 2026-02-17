namespace FlickPickApp.Models;

public class MoviePreference : EntityBase
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string Genre { get; set; }
}
