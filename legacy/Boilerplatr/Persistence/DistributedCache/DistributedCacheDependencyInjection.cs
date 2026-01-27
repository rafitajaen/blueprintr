using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

namespace Boilerplatr.Persistence.DistributedCache;

public static class DistributedCacheDependencyInjection
{
    public static WebApplicationBuilder AddDistributedCache(this WebApplicationBuilder builder, string? connectionString)
    {
        // 1. Verificación: Asegura que el connection string no sea nulo o vacío.
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString, nameof(connectionString));

        // 2. Configuración de FusionCache
        builder.Services.AddFusionCache()
            .WithDefaultEntryOptions(options =>
            {
                options.Duration = TimeSpan.FromMinutes(30);
                
                options.IsFailSafeEnabled = true; 
                options.FailSafeThrottleDuration = TimeSpan.FromSeconds(30); 
            })
            .WithSerializer(new FusionCacheSystemTextJsonSerializer()) 
            .WithDistributedCache(new RedisCache(new RedisCacheOptions
            {
                Configuration = connectionString, 
            }));

        return builder;
    }
}
