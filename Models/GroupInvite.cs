using System.ComponentModel.DataAnnotations;

namespace WatchHive.Models;

public class GroupInvite : EntityBase
{
    [Required]
    public Guid GroupId { get; set; }

    [Required]
    public Guid CreatedById { get; set; }

    [Required]
    public string Token { get; set; } = null!;
    public DateTime? ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }

    // Navigation properties
    public Group Group { get; set; } = null!;
    public User User { get; set; } = null!;
}