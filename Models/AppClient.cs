namespace WatchHive.Models;

public class AppClient
{
    public string ClientId { get; }
    public string ClientSecret { get; }
    public string Name { get; }
    public string ClientURL { get; }

    public AppClient(IConfiguration config)
    {
        ClientId = config["CLIENT_ID"] ?? throw new ArgumentNullException("CLIENT_ID not set");
        ClientSecret = config["CLIENT_SECRET"] ?? throw new ArgumentNullException("CLIENT_SECRET not set");
        Name = config["CLIENT_NAME"] ?? throw new ArgumentNullException("CLIENT_NAME not set");
        ClientURL = config["CLIENT_URL"] ?? throw new ArgumentNullException("CLIENT_URL not set");
    }
}