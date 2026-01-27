using System.IO.Compression;
using System.Reflection;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Boilerplatr.Exceptions;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using NodaTime.Serialization.SystemTextJson;
using NodaTime;

namespace Boilerplatr.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplicationBuilder ConfigureWebHost(this WebApplicationBuilder builder, string filename = "appsettings.json")
    {
        builder.WebHost.UseContentRoot(Directory.GetCurrentDirectory());
        builder.WebHost.UseKestrel(options => builder.Configuration.GetSection("Kestrel").Bind(options));

        /* Configure Settings File */
        builder.Configuration.AddJsonFile
        (
            path: Path.Combine(Directory.GetCurrentDirectory(), "settings", filename),
            optional: true,
            reloadOnChange: true
        );

        builder.Configuration.AddJsonFile
        (
            path: Path.Combine(Directory.GetCurrentDirectory(), "settings", filename.Replace(".json", $".{builder.Environment.EnvironmentName}.json")),
            optional: true,
            reloadOnChange: true
        );

        /* Configure Response Compression */
        builder.Services.AddResponseCompression(o => o.Providers.Add<GzipCompressionProvider>());
        builder.Services.Configure<GzipCompressionProviderOptions>(o => o.Level = CompressionLevel.Fastest);

        /* Configure HttpContextAccessor */
        // CancellationToken (+ info: https://stackoverflow.com/questions/64122616/cancellation-token-injection/77342914#77342914)
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped
        (
            serviceType: typeof(CancellationToken),
            implementationFactory: sp => sp.GetRequiredService<IHttpContextAccessor>().HttpContext?.RequestAborted ?? CancellationToken.None
        );

        builder.Services.Configure<JsonOptions>(options =>
        {
            // Agregar soporte para enums como strings
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());

            // Configurar NodaTime
            options.SerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

            // Opcional: ignorar propiedades nulas
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

        });

        return builder;
    }

    public static WebApplicationBuilder ConfigureCors(this WebApplicationBuilder builder, params string[] origins)
    {
        if (origins.Length == 0)
        {
            builder.Services.AddCors(x => x.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
        }
        else
        {
            builder.Services.AddCors(x => x.AddDefaultPolicy(p => p.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod()));
        }

        return builder;
    }

    public static WebApplicationBuilder AddValidatorsFrom(this WebApplicationBuilder builder, params IEnumerable<Assembly> assemblies)
    {
        builder.Services.AddValidatorsFromAssemblies(assemblies, ServiceLifetime.Singleton, includeInternalTypes: true);

        return builder;
    }

    public static WebApplicationBuilder AddHealthChecks(this WebApplicationBuilder builder)
    {
        var postgres = builder.Configuration.GetConnectionString("Postgresql");
        if (!string.IsNullOrWhiteSpace(postgres))
        {
            builder.Services.AddHealthChecks().AddNpgSql(postgres);
        }

        var redis = builder.Configuration.GetConnectionString("Redis");
        if (!string.IsNullOrWhiteSpace(redis))
        {
            builder.Services.AddHealthChecks().AddRedis(redis);
        }

        return builder;
    }

    public static WebApplicationBuilder AddCustomProblemDetails(this WebApplicationBuilder builder)
    {
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        return builder;
    }
}
