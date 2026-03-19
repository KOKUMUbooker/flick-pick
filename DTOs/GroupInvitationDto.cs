using System.ComponentModel.DataAnnotations;

namespace WatchHive.DTOs;

public class CreateInvitationDto
{
    [Required]
    public string InviteeUserId { get; set; } = null!;

    [Required]
    public string CreatedById { get; set; } = null!;
}