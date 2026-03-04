using WatchHive.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace WatchHive.Services;

public class TokenService : ITokenService
{
    // IConfiguration to access appsettings.json values like issuer and token expiry times
    private readonly IConfiguration _configuration;
    private readonly AppClient _client;

    public TokenService(IConfiguration configuration, AppClient client)
    {
        _configuration = configuration;
        _client = client;
    }

    // Generates a JWT Access Token for the authenticated user.
    public string GenerateAccessToken(User user, RoleEnum role, out string jwtId)
    {
        // Initialize JWT token handler which creates and serializes tokens
        var tokenHandler = new JwtSecurityTokenHandler();

        // Decode the Base64-encoded client secret key into byte array for signing
        var keyBytes = Convert.FromBase64String(_client.ClientSecret);
        var key = new SymmetricSecurityKey(keyBytes);

        // Generate a new unique identifier for the JWT token (jti claim)
        jwtId = Guid.NewGuid().ToString();

        // Read issuer and token expiration from configuration, with default fallback values
        var issuer = _configuration["JwtSettings:Issuer"] ?? "WatchHive";
        var accessTokenExpirationMinutes = int.TryParse(_configuration["JwtSettings:AccessTokenExpirationMinutes"], out var val) ? val : 15;

        // Claims to be embedded in the JWT token
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, jwtId),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Iss, issuer),
            new Claim(JwtRegisteredClaimNames.Aud, _client.ClientURL),
            new Claim("clientId", _client.ClientId),
            new Claim("role", ((int)role).ToString()),
            new Claim("fullName", user.FullName.ToString())
        };

        // Create signing credentials with the symmetric security key and HMAC SHA256 algorithm
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        // Define the JWT token descriptor containing claims, expiration, signing credentials, issuer, and audience
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims), // Claims identity constructed from claims list
            Expires = DateTime.UtcNow.AddMinutes(accessTokenExpirationMinutes), // Token expiration time
            SigningCredentials = creds, // Signing credentials using client secret key
            Issuer = issuer, // Token issuer
            Audience = _client.ClientURL // Token audience (client URL)
        };

        // Create the JWT token based on the descriptor
        var token = tokenHandler.CreateToken(tokenDescriptor);

        // Serialize the JWT token to compact JWT format string
        return tokenHandler.WriteToken(token);
    }

    // Generates a Refresh Token linked to a JWT token and client.
    public RefreshToken GenerateRefreshToken(string ipAddress, string jwtId, Guid userId)
    {
        // Generate a secure random 64-byte array to be used as the refresh token string
        var randomBytes = new byte[64];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        var refreshTokenExpirationDays = int.TryParse(_configuration["JwtSettings:RefreshTokenExpirationDays"], out var val) ? val : 7;

        return new RefreshToken
        {
            Token = Convert.ToBase64String(randomBytes),
            JwtId = jwtId,
            Expires = DateTime.UtcNow.AddDays(refreshTokenExpirationDays),
            UserId = userId,
            ClientId = _client.ClientId,
            IsRevoked = false,
            RevokedAt = null,
            CreatedByIp = ipAddress
        };
    }
}
