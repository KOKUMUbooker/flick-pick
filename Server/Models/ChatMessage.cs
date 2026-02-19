using System.ComponentModel.DataAnnotations;

namespace FlickPickApp.Models;

public class ChatMessage : EntityBase
{
    public Guid MovieNightEventId { get; set; }

    public Guid UserId { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Message { get; set; } = null!;

    // Navigational properties
    public User User { get; set; } = null!;
    public MovieNightEvent MovieNightEvent { get; set; } = null!;
}
