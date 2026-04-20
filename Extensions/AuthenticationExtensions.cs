using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using WatchHive.Services;
using WatchHive.Models;

namespace WatchHive.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        AppClient client)
    {
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

                        if (string.IsNullOrEmpty(clientId)) 
                            return Enumerable.Empty<SecurityKey>();

                        var keyBytes = Convert.FromBase64String(client.ClientSecret);
                        return new[] { new SymmetricSecurityKey(keyBytes) };
                    }
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Cookies["accessToken"];

                        if (!string.IsNullOrEmpty(token))
                        {
                            context.Token = token;
                        }

                        return Task.CompletedTask;
                    },
                    
                    OnTokenValidated = async context =>
                    {
                        var clientId = context.Principal?.FindFirst("clientId")?.Value;

                        if (string.IsNullOrEmpty(clientId))
                        {
                            context.Fail("ClientId claim missing.");
                            return;
                        }

                        if (client.ClientId != clientId)
                        {
                            context.Fail("Invalid client.");
                            return;
                        }

                        var audClaim = context.Principal?.FindFirst(JwtRegisteredClaimNames.Aud)?.Value;

                        if (audClaim != client.ClientURL)
                        {
                            context.Fail("Invalid audience.");
                            return;
                        }

                        // All done successfully
                        await Task.CompletedTask;
                    }
                };
            });

        return services;
    }
}