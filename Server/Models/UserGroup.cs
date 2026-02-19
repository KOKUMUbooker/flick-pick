namespace FlickPickApp.Models;

public class UserGroup
{
    public Guid UserId { get; set; }

    public Guid GroupId { get; set; }

    public bool IsAdmin { get; set; } = false;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    // Navigational properties
    public Group Group { get; set; } = null!;
    public User User { get; set; } = null!;
}
