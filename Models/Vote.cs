namespace FlickPickApp.Models;
using System.ComponentModel.DataAnnotations;

public class Vote : EntityBase
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid MovieSuggestionId { get; set; }

    [Required]
    public VoteType VoteType { get; set; }

   // Navigational properties
    public MovieSuggestion MovieSuggestion { get; set; } = null!;
    public User User { get; set; } = null!;
}

public enum VoteType
{
    Upvote = 1,
    Downvote = 2,
    Veto = 3
}
