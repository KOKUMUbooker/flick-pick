namespace WatchHive.DTOs;

using System.ComponentModel.DataAnnotations;

public class CreateMovieNightDto
{
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