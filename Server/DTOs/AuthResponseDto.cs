namespace FlickPickApp.DTOs;

public class AuthResponseDTO
{
    public string AccessToken { get; set; } = null!;
    public DateTime AccessTokenExpiresAt { get; set; }
    public string? EmailVerificationToken {get; set; } // Will be populated if user is not verified
    public string RefreshToken { get; set; } = null!;
}

public class AuthResult
{
    public AuthResponseDTO? Data { get; set; }
    public AuthErrorType ErrorType { get; set; } = AuthErrorType.None;
    public string ErrorMessage { get; set; } = String.Empty;
}
