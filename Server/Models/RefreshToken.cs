using System.ComponentModel.DataAnnotations;

namespace MovieManager.Models;

public class RefreshToken : EntityBase
{
    // The refresh token string (should be a secure random string)
    [Required]
    public string Token { get; set; } = null!;

    // Helps invalidate refresh tokens when the associated access token is revoked or suspected compromised.
    [Required]
    public string JwtId { get; set; } = null!;

    // Token Expiration Date
    [Required]
    public DateTime Expires { get; set; }

    // Indicates if the token has been revoked
    public bool IsRevoked { get; set; } = false;

    // Date when the token was revoked
    public DateTime? RevokedAt { get; set; }

    // The user associated with the refresh token
    // Foreign key for the associated user
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public string? CreatedByIp { get; set; }

    // Date when the token was created
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    //The client associated with the refresh token
    [Required]
    public Guid ClientId { get; set; }

    public Client Client { get; set; } = null!;
}