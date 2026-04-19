namespace WatchHive.DTOs;

using System.ComponentModel.DataAnnotations;

public class CreateRatingDto
{
    
    [Range(1,5)]
    public int Rating { get; set; }

    [MaxLength(500)]
    public string? Comment { get; set; }

    [Required]
    public string ConnectionId {get; set;} = null!;
}