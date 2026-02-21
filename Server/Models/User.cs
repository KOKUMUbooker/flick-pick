using System.ComponentModel.DataAnnotations;

namespace FlickPickApp.Models;

public class User : EntityBase
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
    public required string Email { get; set; }
    public required string FullName { get; set; }
    public Guid RoleId { get; set; }
    public required string PasswordHash { get; set; }
    public bool EmailVerified { get; set; } = false;
    public string? EmailVerificationToken { get; set; }
    public DateTime? EmailVerificationTokenExpiry { get; set; }
    public string? PasswordResetTokenHash { get; set; }
    public DateTime? PasswordResetTokenExpiry { get; set; }

    // Navigation properties
    public Role Role { get; set; } = null!;
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
    public ICollection<MovieSuggestion> MovieSuggestions { get; set; } = new List<MovieSuggestion>();
    public ICollection<Vote> Votes { get; set; } = new List<Vote>();
    public ICollection<MovieNightRating> MovieNightRatings { get; set; } = new List<MovieNightRating>();
    public ICollection<MovieNightEvent> MovieNightEvents { get; set; } = new List<MovieNightEvent>();
    public ICollection<MoviePreference> MoviePreferences { get; set; } = new List<MoviePreference>();
    public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
    public ICollection<Group> CreatedGroups { get; set; } = new List<Group>();
}
