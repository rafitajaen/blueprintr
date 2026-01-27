using Boilerplatr.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Reflection;

namespace Boilerplatr.OpenTelemetry;

public static class OpenTelemetryDependencyInjection
{
    public static WebApplicationBuilder AddOpenTelemetry(this WebApplicationBuilder builder, Assembly assembly)
    {
        builder.RegisterRequiredOptions<OpenTelemetryOptions, OpenTelemetryOptionsValidation>();

        var options = builder.GetRegisteredOptions<OpenTelemetryOptions>();
        if (options.IsEnabled is not true)
        {
            return builder;
        }

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(builder.Environment.ApplicationName))
            .WithTracing(tracing => tracing
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddNpgsql())
            .WithMetrics(metrics => metrics
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation());

        builder.Logging.AddOpenTelemetry(options =>
        {
            options.IncludeScopes = true;
            options.IncludeFormattedMessage = true;
        });

        builder.Services.AddOpenTelemetry().UseOtlpExporter();


        return builder;
    }
}
