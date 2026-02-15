using FlickPickApp.Models;

namespace FlickPickApp.DTOs;

public class AuthResponseDTO
{
    public string AccessToken { get; set; } = null!;
    public DateTime AccessTokenExpiresAt { get; set; }
    public UIAuthState UserDetails {get; set;} = null!;
    public string RefreshToken { get; set; } = null!;
}

public class AuthResult
{
    public string? EmailVerificationToken {get; set; } // Will be populated if user is not verified
    public AuthResponseDTO? Data { get; set; }
    public AuthErrorType ErrorType { get; set; } = AuthErrorType.None;
    public string ErrorMessage { get; set; } = String.Empty;
}

public class UIAuthState
{
    public string Id { get; set; } = String.Empty;
    public string Email { get; set; } = String.Empty;
    public string Role { get; set; } = String.Empty;
}
