using System.ComponentModel.DataAnnotations;

namespace WatchHive.DTOs;

public class CreateMovieSuggestionDto
{
    [Required]
    public int TmdbId { get; set; }

    [Required]
    public string SuggestedById { get; set; } = null!; 
}