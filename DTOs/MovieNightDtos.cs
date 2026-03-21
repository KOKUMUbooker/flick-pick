namespace WatchHive.DTOs;

using System.ComponentModel.DataAnnotations;

public class UpsertMovieNightDto
{
    public string? Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    [Required]
    public DateTimeOffset ScheduledAt { get; set; }

    [Required]
    public string CreatedById {get; set;} = null!;
}

public class UpdateMovieNightDto
{
    public DateTimeOffset? ScheduledAt { get; set; }
    public bool? IsLocked { get; set; } = false;
    public int? SelectedMovieTmdbId { get; set; }
}

 
public class MovieNightEventDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public DateTimeOffset ScheduledAt { get; set; }
    public bool IsLocked { get; set; }

    public TMDBMovieDto? SelectedMovie { get; set; }

    public double? AverageRating { get; set; }
    public int TotalRatings { get; set; }
}