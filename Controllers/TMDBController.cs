namespace WatchHive.Controllers;

using Microsoft.AspNetCore.Mvc;
using WatchHive.DTOs;
using TMDbLib.Client;

[ApiController]
[Route("/api/tmdb/")]
public class TMDBController : ControllerBase
{
    private readonly TMDbClient _tmdbClient;

    public TMDBController(IConfiguration configuration)
    {
        var apiKey = configuration.GetValue<string>("TMDB:ApiKey") ?? "";
        _tmdbClient = new TMDbClient(apiKey);
    }

    [HttpGet("movie")]
    public async Task<IActionResult> FetchMovieDetails([FromQuery] int tmdbId)
    {
        var movie = await _tmdbClient.GetMovieAsync(tmdbId);
        if (movie == null)
        {
            return NotFound(new CustomError{ Message = "Movie not found" });
        }

        return Ok(new { movie = ToMovieDto(movie)} );
    }

    public static TMDBMovieDto ToMovieDto(TMDbLib.Objects.Movies.Movie movie) 
    {
        var movieDto = new TMDBMovieDto
        {
            Title = movie.Title ?? "",
            Overview = movie.Overview ?? "",
            PosterPath = movie.PosterPath ?? "",
            ReleaseDate = movie.ReleaseDate.ToString() ?? "",
            Runtime = movie.Runtime ?? 0,
            TmdbId = movie.Id
        };

        return movieDto;
    }
}