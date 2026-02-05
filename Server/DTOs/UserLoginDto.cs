using System.ComponentModel.DataAnnotations;

namespace MovieManager.DTOs;

public class UserLoginDto
{
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email provided")]
    public required string Email { get; set; }

    [Required]
    public required string Password {get;set;}

    [Required(ErrorMessage = "ClientId is required.")]
    public string ClientId { get; set; } = null!;
}