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

    public AuthController(MovieAppDbContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
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
            
            return Ok(new { message = "User registered successfully." });
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
}