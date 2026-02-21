namespace WatchHive.Models;

public class Client : EntityBase
{
    public string ClientId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string ClientSecret { get; set; } = null!;
    public string ClientURL { get; set; } = null!;
    public bool IsActive { get; set; } = true;

    // Add navigation property for refresh tokens
    public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
