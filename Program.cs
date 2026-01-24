using Microsoft.EntityFrameworkCore;
using MovieManager.Models;
using MovieManager.Utils;
using MovieManager.Endpoints;
using MovieManager.Services;

namespace MovieManager;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddTransient<IMovieService, MovieService>();

        builder.Services.AddDbContext<MovieDbContext>(options =>
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            options.UseNpgsql(connectionString);
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // SEED DATABASE
        await app.SeedDatabaseAsync();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapMovieEndpoints();

        app.MapGet("/", () => "Hello World!")
        .Produces(200, typeof(string));

        app.Run();
    }
}
