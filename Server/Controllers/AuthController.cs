using Microsoft.AspNetCore.Mvc;
using FlickPickApp.Models;
using FlickPickApp.DTOs;
using Microsoft.EntityFrameworkCore;
using FlickPickApp.Services;
using System.Security.Cryptography;
using System.Text;

namespace FlickPickApp.Controllers;

[ApiController]
[Route("/api/auth/")]
public class AuthController : ControllerBase
{
    private readonly MovieAppDbContext _context;
    private readonly IUserService _userService;
    private IEmailService _emailService;
    private IEmailTemplateService _templateService;
    private IConfiguration _configuration;

    public AuthController(MovieAppDbContext context, IUserService userService,IEmailService emailService,IEmailTemplateService templateService,IConfiguration configuration)
    {
        _context = context;
        _userService = userService;
        _emailService = emailService;
        _templateService = templateService;
        _configuration = configuration;
    }

    [HttpPost("sign-up")]
    public async Task<IActionResult> SignUp([FromBody] RegisterUserDto userDetails)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userExists = await _context.Users.AnyAsync(u => u.Email == userDetails.Email);
        if (userExists)
        {
             return BadRequest("Email is already in use, please log in");
        }
        
        try
        {
            var user = await _userService.CreateUserAsync(userDetails);

            // Send verification email
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var verificationUrl =  $"{baseUrl}/api/auth/verify-email?token={user.EmailVerificationToken}";
            string htmlBody =  await _templateService.GenerateVerificationEmail("FlickPick", user.FullName,verificationUrl);

            await _emailService.SendEmail(user.Email,"Verify email",htmlBody);
            
            return Ok(new { Message = "User registered successfully." ,EmailVerificationToken = user.EmailVerificationToken });
        }
        catch (Exception ex)
        {
            return BadRequest(new CustomError{Message = ex.Message});
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginDto loginDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var result = await _userService.AuthenticateUserAsync(loginDto, ipAddress);

        if (result.ErrorType != AuthErrorType.None || result.Data == null)
        {
            return result.ErrorType switch
            {
                AuthErrorType.InvalidCredentials => Unauthorized(
                    new CustomError { Error = "INVALID_CREDENTIALS", Message = result.ErrorMessage }),

                AuthErrorType.InvalidClient => Unauthorized(
                    new CustomError { Error = "INVALID_CLIENT", Message = result.ErrorMessage }),

                _ => BadRequest(new CustomError { Message = "Authentication failed." })
            };
        }

        // Set refresh token in HTTP-only cookie
        SetAuthTokenCookie(
            "refreshToken",
            result.Data.RefreshToken,
            result.Data.AccessTokenExpiresAt
        );

        SetAuthTokenCookie(
            "accessToken",
            result.Data.AccessToken,
            result.Data.AccessTokenExpiresAt
        );

        return Ok(result.Data);
    }

    // Endpoint to obtain a new access token using a refresh token
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequestDTO refreshRequest)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            var authResponse = await _userService.RefreshTokenAsync(refreshRequest.RefreshToken, refreshRequest.ClientId, ipAddress);

            // If refresh token or client is invalid, return 401 Unauthorized
            if (authResponse == null) return Unauthorized(new CustomError { Message = "Invalid refresh token or client." });

            return Ok(authResponse);
        }
        catch (Exception ex)
        {
            return BadRequest(new CustomError{Message = ex.Message});
        }
    }

    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token)
    {
        if (string.IsNullOrEmpty(token)) return BadRequest(new CustomError { Message = "Token is required." });

        var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailVerificationToken == token);
        
        if (user == null) return BadRequest(new CustomError{ Message = "Invalid verification token." });

        // Email already verified
        if (user.EmailVerified)
        {
            return  BadRequest(new CustomError{ 
                Message = "Email has already been verified, please proceed to login",
             });
        }
        
        if (user.EmailVerificationTokenExpiry < DateTime.UtcNow)
            return BadRequest(new CustomError{ 
                Message = "Verification link has expired.",
                Error = "TOKEN_EXPIRED"
            });
        
        user.EmailVerified = true;
        user.EmailVerificationToken = null;
        user.EmailVerificationTokenExpiry = null;
        await _context.SaveChangesAsync();
        
        // return Ok(new { message = "Email verified successfully." });
        string clientUrl = _configuration.GetValue<string>("UIClient:URL") ?? "http://localhost:5173";
        return Redirect($"{clientUrl}/email-verified?email={user.Email}");
    }

    [HttpPost("resend-verification")]
    public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.EmailVerificationToken == dto.EmailVerificationToken);
        
        if (user == null) return BadRequest(new CustomError{ Message = "Invalid token provided." });
        
        // Generate new token
        user.EmailVerificationToken = _userService.GenerateVerificationToken();
        user.EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(24);
        await _context.SaveChangesAsync();
        
        // Send verification email
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var verificationUrl =  $"{baseUrl}/api/auth/verify-email?token={user.EmailVerificationToken}";
        string htmlBody =  await _templateService.GenerateVerificationEmail("FlickPick", user.FullName,verificationUrl);

        await _emailService.SendEmail(user.Email,"Verify email",htmlBody);
        
        return Ok(new { Message = "Verification email sent." ,EmailVerificationToken = user.EmailVerificationToken });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user != null)
        {
            var rawToken = _userService.GenerateVerificationToken();

            var hashedToken = Convert.ToBase64String(
                SHA256.HashData(Encoding.UTF8.GetBytes(rawToken))
            );

            user.PasswordResetTokenHash = hashedToken;
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddMinutes(30);

            await _context.SaveChangesAsync();

            string clientUrl = _configuration.GetValue<string>("UIClient:URL") ?? "http://localhost:5173";
            var resetUrl = $"{clientUrl}/reset-password?tkn={rawToken}";

            var htmlBody = await _templateService
                .GeneratePasswordResetEmail("FlickPick", user.FullName, resetUrl);

            await _emailService.SendEmail(user.Email, "Password reset", htmlBody);
        }

        return Ok(new { Message = "If an account exists, a reset link has been sent." });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] PasswordResetDto dto)
    {
        var hashedToken = Convert.ToBase64String(
            SHA256.HashData(Encoding.UTF8.GetBytes(dto.PasswordVerificationToken))
        );

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.PasswordResetTokenHash == hashedToken);

        if (user == null)
            return BadRequest(new CustomError { Message = "Invalid or expired token." });

        if (user.PasswordResetTokenExpiry < DateTime.UtcNow)
            return BadRequest(new CustomError { Message = "Invalid or expired token." });

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        user.PasswordResetTokenHash = null;
        user.PasswordResetTokenExpiry = null;

        await _context.SaveChangesAsync();

        await _context.RefreshTokens
            .Where(rt => rt.UserId == user.Id)
            .ExecuteDeleteAsync();

        return Ok(new { Message = "Password reset successfully." });
    }

    private void SetAuthTokenCookie(string cookieName,string refreshToken, DateTime expiresAt)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,               // MUST be true in production
            SameSite = SameSiteMode.None, // TODO: Once figured how to serve UI from backend update this to SameSiteMode.Strict
            Expires = expiresAt
        };

        Response.Cookies.Append(cookieName, refreshToken, cookieOptions);
    }
}