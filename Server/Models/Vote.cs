namespace FlickPickApp.Models;

public class Vote : EntityBase
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid MovieSuggestionId { get; set; }
    public MovieSuggestion MovieSuggestion { get; set; } = null!;

    public VoteType VoteType { get; set; }
}

public enum VoteType
{
    Upvote = 1,
    Downvote = 2,
    Veto = 3
}
