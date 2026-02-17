namespace FlickPickApp.Models;

// Relationship
// One group → many users (via UserGroup)
// One group → many movie nights
public class Group : EntityBase
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    public Guid CreatedById { get; set; }
    public User CreatedBy { get; set; } = null!;

    // Navigational fields
    public ICollection<UserGroup> Members { get; set; } = new List<UserGroup>();
    public ICollection<MovieNightEvent> MovieNightEvents { get; set; } = new List<MovieNightEvent>();
}
