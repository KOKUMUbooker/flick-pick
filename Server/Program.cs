using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MovieManager.Models;
using MovieManager.Services;

namespace MovieManager;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Register services
        builder.Services.AddControllers();
        builder.Services.AddMemoryCache();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddSingleton<IClientCacheService, ClientCacheService>();

        builder.Services.AddDbContext<MovieAppDbContext>(options =>
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            // var dbURL = builder.Configuration["Movies:ConnectionStrings"];
            options.UseNpgsql(connectionString);
        });

        Lazy<IClientCacheService>? clientCacheInstance = null;

        // Configure JWT Bearer Authentication
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // Setup token validation parameters
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,  
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"], 
                    ValidateAudience = false, 
                    ValidateIssuerSigningKey = true, 
                    ValidateLifetime = true,  

                    // fetching the corresponding client’s secret key from cache.
                    IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) =>
                    {
                        // Parse the incoming JWT token to extract claims
                        var jwtToken = new JwtSecurityToken(token);
                        var clientId = jwtToken.Claims.FirstOrDefault(c => c.Type == "client_id")?.Value;

                        if (string.IsNullOrEmpty(clientId) || clientCacheInstance == null)
                            return Enumerable.Empty<SecurityKey>();

                        var client = clientCacheInstance.Value.GetClientByClientIdAsync(clientId).Result;
                        if (client == null) return Enumerable.Empty<SecurityKey>();

                        var keyBytes = Convert.FromBase64String(client.ClientSecret);

                        return new[] { new SymmetricSecurityKey(keyBytes) };
                    }
                };

                // confirming the client exists and audience matches the stored client URL.
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        // Extract client_id claim from the validated token
                        var clientId = context.Principal?.FindFirst("client_id")?.Value;
                        if (string.IsNullOrEmpty(clientId))
                        {
                            // Fail if claim is missing
                            context.Fail("ClientId claim missing.");
                            return;
                        }
                        if(clientCacheInstance == null)
                        {
                            context.Fail("Client Cache Instance is null");
                            return;
                        }
                        // Asynchronously get client info from cache or database
                        var client = await clientCacheInstance.Value.GetClientByClientIdAsync(clientId);
                        if (client == null)
                        {
                            // Fail if client not found
                            context.Fail("Invalid client.");
                            return;
                        }
                        // Extract audience claim from token and compare to client URL stored in DB/cache
                        var audClaim = context.Principal?.FindFirst(JwtRegisteredClaimNames.Aud)?.Value;
                        if (audClaim != client.ClientURL)
                        {
                            // Fail if audience doesn't match
                            context.Fail("Invalid audience.");
                            return;
                        }
                    }
                };
            });


        var app = builder.Build();

        if (app.Environment.IsProduction())
        {
            app.UseHttpsRedirection();
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapControllers();

        app.Run();
    }
}
