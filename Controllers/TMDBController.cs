namespace WatchHive.Controllers;

using WatchHive.DTOs;
using TMDbLib.Client;
using WatchHive.Models;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.General;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
// using System.Text.Json;

[ApiController]
[Route("/api/tmdb/")]
public class TMDBController : ControllerBase
{
    private readonly TMDbClient _tmdbClient;
    private readonly WatchHiveDbContext _dbContext;

    public TMDBController(IConfiguration configuration, WatchHiveDbContext dbContext)
    {
        var apiKey = configuration.GetValue<string>("TMDB:ApiKey") ?? "";
         _tmdbClient = new TMDbClient(apiKey);
        _dbContext = dbContext;
    }

    [HttpGet("movies/search")]
    public async Task<IActionResult> SearchMovies([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest(new CustomError { Message = "Query is required" });

        const int maxResults = 5;

        // 1. Search local DB first
         var dbMovies = await _dbContext.Movies
            // .Where(m => m.Title.ToLower().Contains(query.ToLower()))
            .Where(m => EF.Functions.ILike(m.Title, $"%{query}%"))
            .Take(maxResults)
            .ToListAsync();

         var results = dbMovies
            .Select(MapDbMovie)
            .ToList();

         // 2. If less than desired results → fetch from TMDB
        if (results != null && results.Count < maxResults)
        {
             var search = await _tmdbClient.SearchMovieAsync(query);

            var missing = maxResults - results.Count;
            // Console.WriteLine(JsonSerializer.Serialize(search, new JsonSerializerOptions
            // {
            //     WriteIndented = true
            // }));

             var tmdbResults = search?.Results
                ?.Where(m => !results.Any(r => r.TmdbId == m.Id)) // avoid duplicates
                .Take(missing)
                .Select(m => new TMDBMovieDto
                {
                    TmdbId = m.Id,
                    Title = m.Title ?? "",
                    PosterPath = m?.PosterPath ?? "",
                    ReleaseDate = m?.ReleaseDate?.ToString("yyyy-MM-dd") ?? null,
                    Overview = m?.Overview ?? "",
                    VoteAverage = m?.VoteAverage
                });

            if (tmdbResults != null) results.AddRange(tmdbResults);
        }
 
        var response = new
        {
            page = 1,
            results = results,
            totalPages = 1,
            totalResults = results != null ? results.Count : 0
        };

        return Ok(response);
    }

    private static TMDBMovieDto MapDbMovie(Movie movie)
    {
        return new TMDBMovieDto
        {
            TmdbId = movie.TmdbId,
            Title = movie.Title,
            PosterPath = movie.PosterPath,
            ReleaseDate = movie.ReleaseDate?.ToString("yyyy-MM-dd"),
            Overview = movie.Overview ?? "",
            VoteAverage = movie.VoteAverage ?? 0
        };
    }
}