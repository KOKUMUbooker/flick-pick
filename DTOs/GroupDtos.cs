namespace WatchHive.DTOs;

using System.ComponentModel.DataAnnotations;

public class CreateGroupDto 
{
    [Required]
    public string Name { get; } = null!;
    
    [Required]
    public string UserId { get; } = null!;
    public string? Description { get; }
}

public class ChangeGroupMemberRoleDto
{
    [Required]
    public bool MakeAdmin { get; }
}