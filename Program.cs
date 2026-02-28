using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using dotenv.net;
using WatchHive.Models;
using WatchHive.Services;
using WatchHive.Extensions;
using WatchHive.Hubs;

namespace WatchHive;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddExceptionHandler<AppExceptionHandler>();
        builder.Services.AddProblemDetails();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSignalR();

        if (builder.Environment.IsDevelopment())
        {
            DotEnv.Load();
        }

        builder.Configuration.AddEnvironmentVariables();
        var appClient = new AppClient(builder.Configuration);

        // Register services
        builder.Services.AddControllers();
        builder.Services.AddMemoryCache();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();
        builder.Services.AddSingleton(appClient);

        builder.Services.AddDbContext<WatchHiveDbContext>(options =>
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            options.UseNpgsql(connectionString);
        });

        // Configure JWT Bearer Authentication
        builder.Services.AddJwtAuthentication(builder.Configuration, appClient);

        // Configure a specific CORS policy
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin", policy =>
            {
                policy.WithOrigins(
                    "http://localhost:5173",  // Vite dev server
                    "https://localhost:5173", // Vite with HTTPS
                    "https://productiondomain.com"
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials(); // If using cookies/auth headers
            });
        });        

        var app = builder.Build();
        app.UseExceptionHandler();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseDefaultFiles();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseCors("AllowSpecificOrigin");

        app.MapHub<MovieNightHub>("/movieNightHub");

        app.MapControllers();

        app.MapFallbackToFile("index.html");

        app.Run();
    }
}
