using WatchHive.Models;

namespace WatchHive.DTOs;

using System.ComponentModel.DataAnnotations;
using WatchHive.Models;

public class CreateVoteDto
{
    [Required]
    public string UserId { get; set; } = null!;

    [Required]
    public string ConnectionId { get; set; } = null!;

    [Required]
    public VoteType VoteType { get; set; }
}


public class VoteCountDto
{
    public int UpvoteCount { get; set; }
    public int DownvoteCount { get; set; }
    public VoteType? UserVote { get; set; }
}