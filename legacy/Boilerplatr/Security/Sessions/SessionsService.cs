using Boilerplatr.Abstractions.Entities;
using Boilerplatr.Extensions;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;

namespace Boilerplatr.Security.Sessions;

public interface ISessionsService<TEntity, T> where TEntity : CacheableEntity<TEntity, T>
{
    Task<TEntity?> GetAsync(T id, CancellationToken cancellationToken = default);
    Task SetAsync(TEntity value, HybridCacheEntryOptions? options = null, CancellationToken cancellationToken = default);
    Task RemoveAsync(T id, CancellationToken cancellationToken = default);
}


public class SessionsService
(
    HybridCache cache,
    IOptions<SessionsOptions> sessionOptions
) : ISessionsService<Session, string>
{
    private readonly HybridCache _cache = cache;
    private readonly HybridCacheEntryOptions _entryOptions = new()
    {
        Expiration = TimeSpan.FromMinutes(sessionOptions.Value.SlidingExpirationMinutes),
        LocalCacheExpiration = TimeSpan.FromMinutes(sessionOptions.Value.SlidingExpirationMinutes)
    };

    public async Task<Session?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return default;
        }

        return await _cache.TryGetValueAsync<Session>(Session.GetCacheId(id), cancellationToken);
    }

    public async Task RemoveAsync(string id, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(id))
        {
            await _cache.RemoveAsync(Session.GetCacheId(id), cancellationToken);
        }
    }

    public async Task SetAsync(Session value, HybridCacheEntryOptions? options = null, CancellationToken cancellationToken = default)
    {
        var sessionId = value.GetCacheId();
        if (!string.IsNullOrWhiteSpace(sessionId))
        {
            await _cache.SetAsync(sessionId, value, options ?? _entryOptions, null, cancellationToken);
        }
    }
}
