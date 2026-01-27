using Microsoft.Extensions.Caching.Distributed;
using System.Buffers;
using System.Text.Json;

namespace Boilerplatr.Persistence.DistributedCache;

public interface IDistributedCacheService
{
    Task<T?> GetAsync<T>(string? key, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string? key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
    Task RemoveAsync(string? key, CancellationToken cancellationToken = default);
}

internal sealed class DistributedCacheService(IDistributedCache cache) : IDistributedCacheService
{
    private readonly IDistributedCache _cache = cache;
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(1);

    public async Task<T?> GetAsync<T>(string? key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return default;
        }

        byte[]? bytes = await _cache.GetAsync(key, cancellationToken);

        return bytes is null ? default : Deserialize<T>(bytes);
    }

    public async Task SetAsync<T>(string? key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(key))
        {
            byte[] bytes = Serialize(value);

            await _cache.SetAsync
            (
                key: key,
                value: bytes,
                options: new DistributedCacheEntryOptions
                {
                    SlidingExpiration = expiration ?? DefaultExpiration
                },
                cancellationToken
            );
        }
    }

    public async Task RemoveAsync(string? key, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(key))
        {
            await _cache.RemoveAsync(key, cancellationToken);
        }
    }

    private static T Deserialize<T>(byte[] bytes) => JsonSerializer.Deserialize<T>(bytes)!;

    private static byte[] Serialize<T>(T value)
    {
        var buffer = new ArrayBufferWriter<byte>();
        using var writer = new Utf8JsonWriter(buffer);
        JsonSerializer.Serialize(writer, value);

        return buffer.WrittenSpan.ToArray();
    }
}