using Boilerplatr.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;

namespace Boilerplatr.Security.Sessions;

public static class SessionsDependencyInjection
{
    public static WebApplicationBuilder AddSessions(this WebApplicationBuilder builder)
    {
        builder.RegisterRequiredOptions<SessionsOptions, SessionsOptionsValidation>();

        var sessionOptions = builder.GetRegisteredOptions<SessionsOptions>();

        builder.Services.AddHybridCache(options =>
        {
            options.MaximumPayloadBytes = 1024 * 10 * 10; // 10MB
            options.MaximumKeyLength = 256;
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(sessionOptions.SlidingExpirationMinutes),
                LocalCacheExpiration = TimeSpan.FromMinutes(sessionOptions.SlidingExpirationMinutes)
            };

            options.ReportTagMetrics = true;
            options.DisableCompression = true;
        });

        builder.Services.AddSingleton<ISessionsService<Session, string>, SessionsService>();
        return builder;
    }
}
