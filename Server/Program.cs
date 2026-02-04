using Microsoft.EntityFrameworkCore;
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
        builder.Services.AddScoped<IUserService, UserService>();

        builder.Services.AddDbContext<MovieAppDbContext>(options =>
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            // var dbURL = builder.Configuration["Movies:ConnectionStrings"];
            options.UseNpgsql(connectionString);
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

        app.MapControllers();

        app.Run();
    }
}
