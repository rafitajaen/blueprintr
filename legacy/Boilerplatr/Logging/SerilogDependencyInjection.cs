using Microsoft.AspNetCore.Builder;
using Serilog;

namespace Boilerplatr.Logging;

public static class SerilogDependencyInjection
{
    public static WebApplicationBuilder ConfigureSerilog(this WebApplicationBuilder builder)
    {
        /* Configure Logging */
        // if (Enum.TryParse<LogLevel>(builder.Configuration["Host:LogLevel"], ignoreCase: true, out var logLevel))
        // {
        //     // builder.Logging.ClearProviders();
        //     builder.Logging.SetMinimumLevel(logLevel);
        // }


        builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

        return builder;
    }
}
