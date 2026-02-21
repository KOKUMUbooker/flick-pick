namespace WatchHive.Models;
using System.ComponentModel.DataAnnotations;

public class UserGroup
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid GroupId { get; set; }

    public bool IsAdmin { get; set; } = false;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    // Navigational properties
    public Group Group { get; set; } = null!;
    public User User { get; set; } = null!;
}
