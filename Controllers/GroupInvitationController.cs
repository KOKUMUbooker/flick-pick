namespace WatchHive.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WatchHive.DTOs;
using WatchHive.Models;

[ApiController]
[Route("/api/")]
public class GroupInvitationController : ControllerBase
{
    private readonly WatchHiveDbContext _dbContext;

    public GroupInvitationController(WatchHiveDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("group/invites/{groupId?}")]
    public async Task<IActionResult> GetGroupInvites([FromQuery] Guid userId, [FromRoute] Guid? groupId)
    {
        if (groupId != null)
        {
           var isAdmin = await _dbContext.UserGroups
                .AnyAsync(ug => ug.UserId == userId && ug.GroupId == groupId && ug.IsAdmin);
            if (isAdmin)
            {
                var adminInvitations = await _dbContext.GroupInvitations
                        .Where(gi => gi.GroupId == groupId)
                        .AsNoTracking()
                        .Select(gi => new
                        {
                            Id = gi.Id,
                            Group = new
                            {
                                Name = gi.Group.Name,
                                Description = gi.Group.Description ?? "",
                            },
                            Invitee = new
                            {
                                FullName = gi.Invitee.FullName,
                                Email = gi.Invitee.Email
                            },
                            CreatedBy = new
                            {
                                FullName = gi.CreatedBy.FullName,
                                Email = gi.CreatedBy.Email
                            },
                            Status = gi.Status
                        })
                        .ToListAsync();

                        return Ok(new { invitations = adminInvitations } );
            }
        }

        var invitations = await _dbContext.GroupInvitations
                    .Where(gi => gi.InviteeUserId == userId || gi.CreatedById == userId)
                    .AsNoTracking()
                    .Select(gi => new
                    {
                        Id = gi.Id,
                        Group = new
                        {
                            Name = gi.Group.Name,
                            Description = gi.Group.Description ?? "",
                        },
                        Invitee = new
                        {
                            FullName = gi.Invitee.FullName,
                            Email = gi.Invitee.Email
                        },
                        CreatedBy = new
                        {
                            FullName = gi.CreatedBy.FullName,
                            Email = gi.CreatedBy.Email
                        },
                        Status = gi.Status
                    })
                    .ToListAsync();

        return Ok(new { invitations } );
    }

    [HttpDelete("invitation/{invitationId}")]
    public async Task<IActionResult> DeleteInvitation([FromRoute] Guid invitationId, [FromBody] DeleteInvitationDto deleteDto)
    {
        var invitation = await _dbContext.GroupInvitations.FindAsync(invitationId);
        var message = "Invitation deleted successfully";
        if (invitation == null)
        {
            return Ok(new {message});
        }
        
        if (!Guid.TryParse(deleteDto.Initiator, out Guid parsedInitiator))
        {
            return BadRequest(new CustomError {Message = "Invalid initiator provided"});
        }

        if (invitation.CreatedById != parsedInitiator)
        {
            return BadRequest(new CustomError {Message = "Only the creator of the invitation can delete it"});
        }

        await _dbContext.GroupInvitations
            .Where(gi => gi.Id == invitationId)
            .ExecuteDeleteAsync();

        return Ok(new {message});
    }

    /* On successful processing, the invitation gets deleted */
    [HttpPatch("invitation/{invitationId}/status-update")]
    public async Task<IActionResult> UpdateInvitationStatus([FromRoute] Guid invitationId, [FromBody] UpdateInvitationDto updateDto)
    {
        var invitation = await _dbContext.GroupInvitations.FindAsync(invitationId);
        if (invitation == null)
        {
            return BadRequest(new CustomError{ Message = "The invitation has already been processed. Proceed to confirm your membership in the group" } );
        }

        if (!(updateDto.Status == "cancelled" || updateDto.Status == "approved"  || updateDto.Status == "rejected"))
        {
            return BadRequest(new CustomError{ Message = "You can only cancel or approve an invitation" });
        }

        if (!Guid.TryParse(updateDto.Initiator, out Guid parsedInitiator))
        {
            return BadRequest(new CustomError {Message = "Invalid initiator provided"});
        }

        Guid tempGuid = new();
        Guid parsedGroupId = tempGuid;
        if (!string.IsNullOrEmpty(updateDto.GroupId) && !string.IsNullOrWhiteSpace(updateDto.GroupId))
        {
             if (!Guid.TryParse(updateDto.GroupId, out parsedGroupId))
            {
                return BadRequest(new CustomError {Message = "Invalid groupId provided"});
            }
        }

        var isAdmin = false;
        if (parsedGroupId != tempGuid)
        {
            isAdmin = await _dbContext.UserGroups
               .AnyAsync(ug => ug.UserId == parsedInitiator && ug.GroupId == parsedGroupId && ug.IsAdmin);
        }

        // if initiator is not the creator of the invitation or the invitation invitee or not the group admin, cancel request
        bool areInvitationParties = invitation.CreatedById == parsedInitiator ||  invitation.InviteeUserId == parsedInitiator;
        if (!areInvitationParties && !isAdmin)
        {
            return BadRequest(new CustomError { Message = "You are not authorized to update this invitation" } );
        } 

        if (invitation.Status == "cancelled")
        {
            return BadRequest(new CustomError{ Message = "Invitation has already been cancelled" });
        }

        if (updateDto.Status == "approved")
        {
            // Create member entry for the user
            var membership = new UserGroup
            {
                GroupId = invitation.GroupId,
                UserId = invitation.InviteeUserId,
            };

            await _dbContext.UserGroups.AddAsync(membership);
       
            await _dbContext.GroupInvitations
                .Where(gi => gi.Id == invitationId)
                .ExecuteDeleteAsync();
            
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Invitation accepted successfully" });
        } 
        else // status == "cancelled"
        {
            await _dbContext.GroupInvitations
                .Where(gi => gi.Id == invitationId)
                .ExecuteDeleteAsync();

            return Ok(new { message = "Invitation cancelled successfully" });
        }
    }

    [HttpPost("group/{groupId}/invite")]
    public async Task<IActionResult> CreateGroupInvitation([FromRoute] string groupId, [FromBody] CreateInvitationDto inviteDto)
    {
        if (!Guid.TryParse(groupId, out Guid parsedGroupId))
        {
            return BadRequest(new CustomError {Message = "Invalid group id provided"});
        }

        if (!Guid.TryParse(inviteDto.InviteeUserId, out Guid parsedInviteeUserId)) 
        {
            return BadRequest(new CustomError {Message = "Invalid invitee user id provided"});
        }

        if (!Guid.TryParse(inviteDto.CreatedById, out Guid parsedCreatedById)) 
        {
            return BadRequest(new CustomError {Message = "Invalid creator id provided"});
        }

        var groupExists = await _dbContext.Groups.AnyAsync(g => g.Id == parsedGroupId);
        if (!groupExists) return NotFound(new CustomError {Message="The group does not exist"} );
        
        var inviteeExists = await _dbContext.Users.AnyAsync(u => u.Id == parsedInviteeUserId);
        if (!inviteeExists) return NotFound(new CustomError {Message="The invitee does not exist"} );
        
        if (parsedInviteeUserId != parsedCreatedById)
        {
            var creatorExists = await _dbContext.Users.AnyAsync(u => u.Id == parsedCreatedById);
            if (!creatorExists) return NotFound(new CustomError {Message="The request creator does not exist"} );
        }

        // Check for an unresolved group invitation
        var pendingInvitationExists = await _dbContext.GroupInvitations
                    .AnyAsync(gi => gi.InviteeUserId == parsedInviteeUserId 
                                    && gi.GroupId == parsedGroupId
                                    && gi.Status == "pending");
        if (pendingInvitationExists)
        {
            return BadRequest(new CustomError{Message = "An earlier invitation had been sent. No new invitations allowed until the initial is resolved" });
        }

        // Check if user is already a member
        var isAlreadyAMember = await _dbContext.UserGroups
                .AnyAsync(m => m.GroupId == parsedGroupId && m.UserId == parsedInviteeUserId);
        if (isAlreadyAMember)
        {
            return BadRequest(new CustomError{Message="The user is already a member in the group"});
        }

        var invitation = new GroupInvitation
        {
            InviteeUserId = parsedInviteeUserId,
            CreatedById = parsedCreatedById,
            GroupId = parsedGroupId
        };

        await _dbContext.GroupInvitations.AddAsync(invitation);

        await _dbContext.SaveChangesAsync();

        var Message =  "Group invitation created successfully";
        if (parsedCreatedById == parsedInviteeUserId) Message = "Join request created successfully";

        return Ok(new { Message , InvitationId = invitation.Id });
    }
}