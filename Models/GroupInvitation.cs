using System.ComponentModel.DataAnnotations;

namespace WatchHive.Models;

// <summary>
//  Allows for admins to send invite requests to potential members
//  Can also be used by regular users to send join requests to some group so that an admin can approve
//  If its a join request - the InviteeUserId and the CreatedById will be the same
// <summary/>
public class GroupInvitation : EntityBase
{
    [Required]
    public Guid GroupId { get; set; }

    [Required]
    public Guid InviteeUserId { get; set; }

    [Required]
    public Guid CreatedById { get; set; }

    public string Status { get; set; } = "pending"; // "pending" | "approved" | "cancelled"

    // Navigation properties
    public Group Group { get; set; } = null!;
    public User Invitee { get; set; } = null!;
    public User CreatedBy { get; set; } = null!;
}