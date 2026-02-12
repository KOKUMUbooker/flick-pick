namespace MovieManager.DTOs;

public class ForgotPasswordDto 
{
   public string Email { get; set; }
}

public class PasswordResetDto 
{
    public string PasswordVerificationToken { get; set; }
    public string NewPassword { get; set; }
}