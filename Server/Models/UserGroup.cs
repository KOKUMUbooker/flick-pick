namespace FlickPickApp.Models;

public class UserGroup
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid GroupId { get; set; }
    public Group Group { get; set; } = null!;

    public bool IsAdmin { get; set; } = false;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}
