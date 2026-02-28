namespace WatchHive.DTOs;

using System.ComponentModel.DataAnnotations;

public class CreateGroupDto 
{
    [Required]
    public string Name { get; set; } = null!;
    
    [Required]
    public string UserId { get; set; } = null!;
    public string? Description { get; set; }
}