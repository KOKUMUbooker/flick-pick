using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WatchHive.DTOs;
using WatchHive.Models;

namespace WatchHive.Services;

public class UserService : IUserService
{
    private readonly WatchHiveDbContext _dbContext;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;
    private readonly AppClient _client;

    public UserService(WatchHiveDbContext dbContext, ITokenService tokenService, IConfiguration configuration, AppClient client)
    {
        _dbContext = dbContext;
        _tokenService = tokenService;
        _configuration = configuration;
        _client = client;
    }

    public async Task<User> CreateUserAsync(RegisterUserDto userDto, Guid? roleId = null)
    {
        // Get role
        Guid finalRoleId;

        if (roleId.HasValue)
        {
            finalRoleId = roleId.Value;
        }
        else
        {
            var defaultRole = await _dbContext.Roles
                .FirstOrDefaultAsync(r => r.RoleValue == RoleEnum.User);
            finalRoleId = defaultRole?.Id ?? throw new Exception("Default role not found");
        }

        // Create user
        var user = new User
        {
            Email = userDto.Email,
            FullName = userDto.FullName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
            RoleId = finalRoleId,
            EmailVerificationToken = GenerateVerificationToken(),
            EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(24)
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return user;
    }

    public UserDetailsDto? GetUserFromAccessToken(string token)
    {
        if (string.IsNullOrEmpty(token)) return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var keyBytes = Convert.FromBase64String(_client.ClientSecret);
        var key = new SymmetricSecurityKey(keyBytes);

        try
        {
            var claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _configuration["JwtSettings:Issuer"] ?? "WatchHive",
                ValidateAudience = true,
                ValidAudience = _client.ClientURL,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(30) // optional small clock skew
            }, out SecurityToken validatedToken);

            // Extract claims
            var userId = claimsPrincipal.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var email = claimsPrincipal.FindFirstValue(JwtRegisteredClaimNames.Email);
            var fullName = claimsPrincipal.FindFirstValue("fullName");
            var role = claimsPrincipal.FindFirstValue("role");

            if (userId == null || email == null || fullName == null || role == null) return null;

            return new UserDetailsDto
            {
                Id = userId,
                Email = email,
                FullName = fullName,
                Role = role
            };
        }
        catch
        {
            // token invalid or expired
            return null;
        }
    }

    public async Task<AuthResult> AuthenticateUserAsync(UserLoginDto loginDto, string ipAddress)
    {
        var user = await _dbContext.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            return new AuthResult
            {
                ErrorType = AuthErrorType.InvalidCredentials,
                ErrorMessage = "Invalid credentials."
            };
        }

        if (!user.EmailVerified)
        {
            return new AuthResult
            {
                EmailVerificationToken = user.EmailVerificationToken,
                ErrorType = AuthErrorType.EmailNotVerified,
                ErrorMessage = "Email not verified.",
            };
        }

        if (_client.ClientId != loginDto.ClientId)
        {
            return new AuthResult
            {
                ErrorType = AuthErrorType.InvalidClient,
                ErrorMessage = "Invalid client."
            };
        }

        var accessToken = _tokenService.GenerateAccessToken(user, user.Role.RoleValue, out string jwtId);
        var refreshToken = _tokenService.GenerateRefreshToken(ipAddress, jwtId, user.Id);

        _dbContext.RefreshTokens.Add(refreshToken);
        await _dbContext.SaveChangesAsync();

        return new AuthResult
        {
            Data = new AuthResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(15),
                UserDetails = new UserDetailsDto
                {
                    Id = user.Id.ToString(),
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = ((int)user.Role.RoleValue).ToString() // To get the numeric value & not the key
                }
            }
        };
    }


    // Refreshes an expired access token using a valid refresh token and client ID
    public async Task<AuthResponseDTO?> RefreshTokenAsync(string refreshToken, string clientId, string ipAddress)
    {
        if (clientId != _client.ClientId)
        {
            return null;
        }

        // Look up the refresh token in database, including related user and their role for new token generation
        var existingToken = await _dbContext.RefreshTokens
            .Include(rt => rt.User)
            .ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.ClientId == clientId);

        // Validate refresh token existence, revocation status, and expiration
        if (existingToken == null || existingToken.IsRevoked || existingToken.Expires <= DateTime.UtcNow)
            return null; // Invalid refresh token

        // Revoke old refresh token immediately to prevent reuse
        existingToken.IsRevoked = true;
        existingToken.RevokedAt = DateTime.UtcNow;

        var user = existingToken.User;
        // Generate a new access token with fresh JWT ID and client info
        var accessToken = _tokenService.GenerateAccessToken(user, user.Role.RoleValue, out string newJwtId);
        // Generate a new refresh token linked to the new JWT ID
        var newRefreshToken = _tokenService.GenerateRefreshToken(ipAddress, newJwtId, user.Id);

        // Store the new refresh token in the database
        _dbContext.RefreshTokens.Add(newRefreshToken);
        await _dbContext.SaveChangesAsync();

        // Read access token expiration duration from config or default to 15 minutes
        var accessTokenExpiryMinutes = int.TryParse(_configuration["JwtSettings:AccessTokenExpirationMinutes"], out var val) ? val : 15;

        // Return the new tokens and expiry info
        return new AuthResponseDTO
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken.Token,
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(accessTokenExpiryMinutes),
            UserDetails = new UserDetailsDto
            {
                Id = user.Id.ToString(),
                Email = user.Email,
                FullName = user.FullName,
                Role = ((int)user.Role.RoleValue).ToString() // To get the numeric value & not the key
            }
        };
    }

    // Revokes an existing refresh token to prevent further use
    public async Task<bool> RevokeRefreshTokenAsync(string refreshToken, string ipAddress)
    {
        // Look up the refresh token in the database
        var existingToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        // Return false if token not found or already revoked
        if (existingToken == null || existingToken.IsRevoked)
            return false;

        // Mark token as revoked and record revocation time
        existingToken.IsRevoked = true;
        existingToken.RevokedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return true; // Indicate successful revocation
    }

    public string GenerateVerificationToken()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }
}
