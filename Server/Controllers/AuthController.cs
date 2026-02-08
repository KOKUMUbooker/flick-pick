using Microsoft.AspNetCore.Mvc;
using MovieManager.Models;
using MovieManager.DTOs;
using Microsoft.EntityFrameworkCore;
using MovieManager.Services;

namespace MovieManager.Controllers;

[ApiController]
[Route("/api/auth/")]
public class AuthController : ControllerBase
{
    private readonly MovieAppDbContext _context;
    private readonly IUserService _userService;
    private IEmailService _emailService;
    private IEmailTemplateService _templateService;

    public AuthController(MovieAppDbContext context, IUserService userService,IEmailService emailService,IEmailTemplateService templateService)
    {
        _context = context;
        _userService = userService;
        _emailService = emailService;
        _templateService = templateService;
    }

    [HttpPost("sign-up")]
    public async Task<IActionResult> SignUp([FromBody] RegisterUserDto userDetails)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userExists = await _context.Users.AnyAsync(u => u.Email == userDetails.Email);
        if (userExists) return BadRequest("Email is already in use.");
        
        try
        {
            var user = await _userService.CreateUserAsync(userDetails);

            // Send verification email
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var verificationUrl =  $"{baseUrl}/api/auth/verify-email?token={user.EmailVerificationToken}";
            string htmlBody =  await _templateService.GenerateVerificationEmail("Movie-Manager", user.FullName,verificationUrl);

            await _emailService.SendEmail(user.Email,"Verify email",htmlBody);
            
            return Ok(new { message = "User registered successfully." ,emailVerificationToken = user.EmailVerificationToken });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginDto loginDto)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            var authResponse = await _userService.AuthenticateUserAsync(loginDto, ipAddress);
            
            // If authentication fails (invalid credentials or client), return 401 Unauthorized
            if (authResponse == null) return Unauthorized(new { message = "Invalid credentials or client." });
            
            return Ok(authResponse);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
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
            if (authResponse == null) return Unauthorized(new { message = "Invalid refresh token or client." });

            return Ok(authResponse);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token)
    {
        if (string.IsNullOrEmpty(token)) return BadRequest(new { message = "Token is required." });

        var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailVerificationToken == token);
        
        if (user == null) return BadRequest(new { message = "Invalid verification token." });
        
        if (user.EmailVerificationTokenExpiry < DateTime.UtcNow)
            return BadRequest(new { 
                message = "Verification link has expired.",
                code = "TOKEN_EXPIRED"
            });
        
        user.EmailVerified = true;
        user.EmailVerificationToken = null;
        user.EmailVerificationTokenExpiry = null;
        await _context.SaveChangesAsync();
        
        return Ok(new { message = "Email verified successfully." });
    }

    [HttpPost("resend-verification")]
    public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.EmailVerificationToken == dto.Token);
        
        if (user == null)
            return BadRequest(new { message = "Invalid token." });
        
        // Generate new token
        user.EmailVerificationToken = _userService.GenerateVerificationToken();
        user.EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(24);
        await _context.SaveChangesAsync();
        
        // Send email (implement your email service)
        // await _emailService.SendVerificationEmail(user.Email, user.EmailVerificationToken);
        
        return Ok(new { message = "Verification email sent." });
    }
}