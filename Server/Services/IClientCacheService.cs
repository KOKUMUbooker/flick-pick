using MovieManager.Models;
namespace MovieManager.Services;

public interface IClientCacheService
{
    // Async method: fetch from cache or DB if missing and update cache
    Task<Client?> GetClientByClientIdAsync(string clientId);
}