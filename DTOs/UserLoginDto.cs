using System.ComponentModel.DataAnnotations;

namespace WatchHive.DTOs;

public class UserLoginDto
{
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email provided")]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }

    [Required(ErrorMessage = "ClientId is required.")]
    public required string ClientId { get; set; } = null!;
}

public class ResendVerificationDto
{
    public string EmailVerificationToken { get; set; } = string.Empty;
}
