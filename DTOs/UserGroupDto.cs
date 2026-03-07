namespace WatchHive.DTOs;

public class UserGroupDto
{
    public string Id { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime JoinedAt { get; set; }
    public bool IsAdmin { get; set; }
}