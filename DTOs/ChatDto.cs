using System.ComponentModel.DataAnnotations;

namespace WatchHive.DTOs;

public class CreateChatMsgDto
{
    [Required]
    public string UserId { get; set; } = null!;

    [Required]
    public string ConnectionId { get; set; } = null!;

    [Required]
    [MaxLength(1000)]
    public string Message { get; set; } = null!;
    public DateTime SentAt { get; set; } 
}