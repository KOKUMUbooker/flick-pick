namespace FlickPickApp.Models;

public class Vote : EntityBase
{
    public Guid UserId { get; set; }

    public Guid MovieSuggestionId { get; set; }

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
