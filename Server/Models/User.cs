using System.ComponentModel.DataAnnotations;

namespace MovieManager.Models;

public class User : EntityBase
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
    public required string Email { get; set; }

    [MinLength(3, ErrorMessage = "Full name must be at least 3 characters long")]
    public required string FullName { get; set; }

    [Required]
    public Guid RoleId { get; set; }

    public bool EmailVerified {get; set;} = false;

    [Required]
    public required string PasswordHash { get; set; }
    
    // Navigation properties
    public Role Role { get; set; } = null!;
    public List<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
