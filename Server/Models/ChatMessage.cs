namespace FlickPickApp.Models;

public class ChatMessage : EntityBase
{
    public Guid MovieNightEventId { get; set; }
    public MovieNightEvent MovieNightEvent { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    [Required]
    [MaxLength(1000)]
    public string Message { get; set; } = null!;
}
