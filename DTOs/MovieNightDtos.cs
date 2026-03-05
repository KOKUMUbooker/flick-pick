namespace WatchHive.DTOs;

using System.ComponentModel.DataAnnotations;

public class CreateMovieNightDto
{
    [Required]
    public string Name { get; set; }

    public string? Description { get; set; }

    [Required]
    public DateTime ScheduledAt { get; set; }

    [Required]
    public string CreatedById {get; set;} = null!;
}

public class UpdateMovieNightDto
{
    public DateTime? ScheduledAt { get; set; }
    public bool? IsLocked { get; set; } = false;
    public int? SelectedMovieTmdbId { get; set; }
}