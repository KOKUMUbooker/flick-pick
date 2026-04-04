using System.ComponentModel.DataAnnotations;

namespace WatchHive.DTOs;

public class CreateChatMsgDto
{
    public string UserId { get; set; } = null!;

    [Required]
    [MaxLength(1000)]
    public string Message { get; set; } = null!;
    public DateTime SentAt { get; set; } 
}