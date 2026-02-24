using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using WatchHive.Services;

namespace WatchHive.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        Lazy<IClientCacheService>? clientCacheInstance = null;

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,

                    IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) =>
                    {
                        var jwtToken = new JwtSecurityToken(token);
                        var clientId = jwtToken.Claims
                            .FirstOrDefault(c => c.Type == "clientId")?.Value;

                        if (string.IsNullOrEmpty(clientId) || clientCacheInstance == null)
                            return Enumerable.Empty<SecurityKey>();

                        var client = clientCacheInstance.Value
                            .GetClientByClientIdAsync(clientId).Result;

                        if (client == null)
                            return Enumerable.Empty<SecurityKey>();

                        var keyBytes = Convert.FromBase64String(client.ClientSecret);
                        return new[] { new SymmetricSecurityKey(keyBytes) };
                    }
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var clientId = context.Principal?
                            .FindFirst("clientId")?.Value;

                        if (string.IsNullOrEmpty(clientId))
                        {
                            context.Fail("ClientId claim missing.");
                            return;
                        }

                        var cache = context.HttpContext.RequestServices
                            .GetRequiredService<IClientCacheService>();

                        var client = await cache
                            .GetClientByClientIdAsync(clientId);

                        if (client == null)
                        {
                            context.Fail("Invalid client.");
                            return;
                        }

                        var audClaim = context.Principal?
                            .FindFirst(JwtRegisteredClaimNames.Aud)?.Value;

                        if (audClaim != client.ClientURL)
                        {
                            context.Fail("Invalid audience.");
                        }
                    }
                };
            });

        return services;
    }
}