using System.ComponentModel.DataAnnotations;

namespace FlickPickApp.Models;

public class RefreshToken : EntityBase
{
    // The refresh token string (should be a secure random string)
    [Required]
    public string Token { get; set; } = null!;

    // Helps invalidate refresh tokens when the associated access token is revoked or suspected compromised.
    [Required]
    public string JwtId { get; set; } = null!;

    [Required]
    public DateTime Expires { get; set; }
    public bool IsRevoked { get; set; } = false;
    public DateTime? RevokedAt { get; set; }
    public Guid UserId { get; set; }
    public string? CreatedByIp { get; set; }

    [Required]
    public Guid ClientId { get; set; }

    // Navigational properties
    public User User { get; set; } = null!;
    public Client Client { get; set; } = null!;
}
