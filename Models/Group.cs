using System.ComponentModel.DataAnnotations;

namespace WatchHive.Models;

// Relationship
// One group → many users (via UserGroup)
// One group → many movie nights
public class Group : EntityBase
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;
    public string? Description { get; set; } 

    public Guid CreatedById { get; set; }

    // Navigational fields
    public User CreatedBy { get; set; } = null!;
    public ICollection<UserGroup> Members { get; set; } = new List<UserGroup>();
    public ICollection<MovieNightEvent> MovieNightEvents { get; set; } = new List<MovieNightEvent>();
    public ICollection<GroupInvite> GroupInvites { get; set; } = new List<GroupInvite>();
}
