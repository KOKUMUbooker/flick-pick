using MovieManager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace MovieManager.Services;

    public class ClientCacheService : IClientCacheService
    {
        // Prefix to uniquely identify each client entry in the cache
        private const string CacheKeyPrefix = "Client_";

        // Service provider to create scopes for scoped services like DbContext
        private readonly IServiceProvider _serviceProvider;

        // Memory cache instance for storing cached client data in-memory
        private readonly IMemoryCache _memoryCache;

        public ClientCacheService(IServiceProvider serviceProvider, IMemoryCache memoryCache)
        {
            _serviceProvider = serviceProvider;
            _memoryCache = memoryCache;
        }

        public async Task<Client?> GetClientByClientIdAsync(string clientId)
        {
            var cacheKey = CacheKeyPrefix + clientId;

            if (_memoryCache.TryGetValue<Client>(cacheKey, out var client))
            {
                return client;
            }

            // Cache miss - create a new scope to get a fresh DbContext instance
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<MovieAppDbContext>();

            client = await dbContext.Clients.AsNoTracking()
                .FirstOrDefaultAsync(c => c.ClientId == clientId && c.IsActive);

            if (client != null)
            {
                _memoryCache.Set(cacheKey, client, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                });
            }

            return client;
        }
    }