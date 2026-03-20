using System.ComponentModel.DataAnnotations;

namespace WatchHive.DTOs;

public class CreateInvitationDto
{
    [Required]
    public string InviteeUserId { get; set; } = null!;

    [Required]
    public string CreatedById { get; set; } = null!;
}

public class UpdateInvitationDto
{
    [Required]
    public string Initiator { get; set; } = null!;

    [Required]
    public string Status { get; set; } = null!;
}

public class DeleteInvitationDto
{
    [Required]
    public string Initiator { get; set; } = null!;
}