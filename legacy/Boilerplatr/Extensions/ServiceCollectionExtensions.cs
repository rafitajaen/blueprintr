using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Boilerplatr.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBackgroundHostedService<TService, TImplementation>
    (
        this IServiceCollection services
    )
    where TService : class, IHostedService
    where TImplementation : class, TService
    {
        return services
            .AddSingleton<TImplementation>()
            .AddSingleton<TService>(c => c.GetRequiredService<TImplementation>())
            .AddHostedService<TService>(c => c.GetRequiredService<TImplementation>());
    }
}
