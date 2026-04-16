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

    // public string? ConnectionId { get; set; }
}

public class UpdateMovieNightDto
{
    [Required]
    public string? Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTimeOffset? ScheduledAt { get; set; }

    public string? ConnectionId { get; set; }
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

public class ComputeEventResultDto 
{
    [Required]
    public string Initiator { get; set; } = null!;
    
    [Required]
    public string ConnectionId { get; set; } = null!;
}

public class DeleteMovieEventEventDto 
{
    [Required]
    public string Initiator { get; set; } = null!;
    
    [Required]
    public string ConnectionId { get; set; } = null!;
}