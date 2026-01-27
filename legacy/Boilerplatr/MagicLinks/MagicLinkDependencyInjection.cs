using Boilerplatr.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Boilerplatr.MagicLinks;

public static class VerificationLinkDependencyInjection
{
    public static WebApplicationBuilder AddMagicLink<TContext>(this WebApplicationBuilder builder) where TContext : DbContext
    {
        /* Magic Token */
        builder.RegisterRequiredOptions<MagicLinkOptions, MagicLinkOptionsValidation>();

        builder.Services.AddSingleton<MagicLinkTemplate>();
        builder.Services.AddBackgroundHostedService<IMagicLinkService<TContext>, MagicLinkService<TContext>>();

        return builder;
    }
}
