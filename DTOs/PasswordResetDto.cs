namespace FlickPickApp.DTOs;

public class ForgotPasswordDto
{
    public required string Email { get; set; }
}

public class PasswordResetDto
{
    public required string PasswordVerificationToken { get; set; }
    public required string NewPassword { get; set; }
}
