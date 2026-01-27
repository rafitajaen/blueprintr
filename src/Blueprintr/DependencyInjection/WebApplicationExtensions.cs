using System.IO.Compression;
using System.Reflection;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using System.Text.Json.Serialization;

namespace Blueprintr.DependencyInjection;

/// <summary>
/// Provides extension methods for configuring ASP.NET Core web applications.
/// </summary>
/// <remarks>
/// This class contains extension methods that simplify common web application configuration tasks
/// including web host setup, CORS policies, validation, health checks, and exception handling.
/// Added in version 1.0.0.
/// </remarks>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Configures the web host with common settings including Kestrel, configuration files, response compression, and JSON serialization.
    /// </summary>
    /// <param name="builder">The web application builder to configure.</param>
    /// <param name="settingsFilename">The name of the settings file to load. Defaults to "appsettings.json".</param>
    /// <param name="settingsDirectory">The directory containing settings files. Defaults to "settings".</param>
    /// <param name="compressionLevel">The compression level for Gzip responses. Defaults to <see cref="CompressionLevel.Fastest"/>.</param>
    /// <param name="configureJsonOptions">Optional action to configure JSON serialization options beyond the defaults.</param>
    /// <returns>The configured web application builder for method chaining.</returns>
    /// <remarks>
    /// This method performs the following configuration:
    /// - Sets the content root to the current directory
    /// - Binds Kestrel configuration from the "Kestrel" section
    /// - Loads settings from the specified file and environment-specific variant
    /// - Configures Gzip response compression
    /// - Registers HttpContextAccessor and CancellationToken for dependency injection
    /// - Configures JSON serialization with enum string conversion, NodaTime support, and null value handling
    /// Added in version 1.0.0.
    /// </remarks>
    public static WebApplicationBuilder ConfigureWebHost(
        this WebApplicationBuilder builder,
        string settingsFilename = "appsettings.json",
        string settingsDirectory = "settings",
        CompressionLevel compressionLevel = CompressionLevel.Fastest,
        Action<JsonOptions>? configureJsonOptions = null)
    {
        builder.WebHost.UseContentRoot(Directory.GetCurrentDirectory());
        builder.WebHost.UseKestrel(options => builder.Configuration.GetSection("Kestrel").Bind(options));

        /* Configure Settings File */
        builder.Configuration.AddJsonFile
        (
            path: Path.Combine(Directory.GetCurrentDirectory(), settingsDirectory, settingsFilename),
            optional: true,
            reloadOnChange: true
        );

        builder.Configuration.AddJsonFile
        (
            path: Path.Combine(Directory.GetCurrentDirectory(), settingsDirectory, settingsFilename.Replace(".json", $".{builder.Environment.EnvironmentName}.json")),
            optional: true,
            reloadOnChange: true
        );

        /* Configure Response Compression */
        builder.Services.AddResponseCompression(o => o.Providers.Add<GzipCompressionProvider>());
        builder.Services.Configure<GzipCompressionProviderOptions>(o => o.Level = compressionLevel);

        /* Configure HttpContextAccessor */
        // CancellationToken (+ info: https://stackoverflow.com/questions/64122616/cancellation-token-injection/77342914#77342914)
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped
        (
            serviceType: typeof(CancellationToken),
            implementationFactory: sp => sp.GetRequiredService<IHttpContextAccessor>().HttpContext?.RequestAborted ?? CancellationToken.None
        );

        /* Configure JSON Serialization */
        builder.Services.Configure<JsonOptions>(options =>
        {
            // Add support for enums as strings
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());

            // Configure NodaTime
            options.SerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

            // Ignore null properties when writing
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

            // Apply additional custom configuration if provided
            configureJsonOptions?.Invoke(options);
        });

        return builder;
    }

    /// <summary>
    /// Configures Cross-Origin Resource Sharing (CORS) policies for the application.
    /// </summary>
    /// <param name="builder">The web application builder to configure.</param>
    /// <param name="origins">
    /// Optional array of allowed origins. If empty, allows any origin.
    /// If specified, only the listed origins will be allowed.
    /// </param>
    /// <returns>The configured web application builder for method chaining.</returns>
    /// <remarks>
    /// When no origins are specified, the policy allows any origin, header, and method (useful for development).
    /// When origins are specified, only those origins are allowed, but any header and method are still permitted.
    /// Added in version 1.0.0.
    /// </remarks>
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

    /// <summary>
    /// Registers FluentValidation validators from the specified assemblies.
    /// </summary>
    /// <param name="builder">The web application builder to configure.</param>
    /// <param name="assemblies">The assemblies to scan for validator types.</param>
    /// <param name="lifetime">
    /// The service lifetime for registered validators. Defaults to <see cref="ServiceLifetime.Singleton"/>.
    /// </param>
    /// <param name="includeInternalTypes">
    /// Whether to include internal validator types. Defaults to true.
    /// </param>
    /// <returns>The configured web application builder for method chaining.</returns>
    /// <remarks>
    /// This method scans the provided assemblies for types implementing FluentValidation's IValidator interface
    /// and registers them with the dependency injection container.
    /// Added in version 1.0.0.
    /// </remarks>
    public static WebApplicationBuilder AddValidatorsFrom
    (
        this WebApplicationBuilder builder,
        IEnumerable<Assembly> assemblies,
        ServiceLifetime lifetime = ServiceLifetime.Singleton,
        bool includeInternalTypes = true
    )
    {
        builder.Services.AddValidatorsFromAssemblies(assemblies, lifetime, includeInternalTypes: includeInternalTypes);

        return builder;
    }

    /// <summary>
    /// Configures health checks for PostgreSQL and Redis connections if connection strings are available.
    /// </summary>
    /// <param name="builder">The web application builder to configure.</param>
    /// <param name="postgresConnectionStringKey">
    /// The configuration key for the PostgreSQL connection string. Defaults to "Postgresql".
    /// </param>
    /// <param name="redisConnectionStringKey">
    /// The configuration key for the Redis connection string. Defaults to "Redis".
    /// </param>
    /// <returns>The configured web application builder for method chaining.</returns>
    /// <remarks>
    /// This method checks for PostgreSQL and Redis connection strings in the configuration.
    /// If found, it registers the corresponding health checks. If a connection string is not found
    /// or is empty, that health check is skipped.
    /// Added in version 1.0.0.
    /// </remarks>
    public static WebApplicationBuilder AddHealthChecks
    (
        this WebApplicationBuilder builder,
        string postgresConnectionStringKey = "Postgresql",
        string redisConnectionStringKey = "Redis"
    )
    {
        var postgres = builder.Configuration.GetConnectionString(postgresConnectionStringKey);
        if (!string.IsNullOrWhiteSpace(postgres))
        {
            builder.Services.AddHealthChecks().AddNpgSql(postgres);
        }

        var redis = builder.Configuration.GetConnectionString(redisConnectionStringKey);
        if (!string.IsNullOrWhiteSpace(redis))
        {
            builder.Services.AddHealthChecks().AddRedis(redis);
        }

        return builder;
    }

    /// <summary>
    /// Registers custom problem details handling with a global exception handler.
    /// </summary>
    /// <typeparam name="TExceptionHandler">
    /// The type of the exception handler to register. Must implement <see cref="IExceptionHandler"/>.
    /// </typeparam>
    /// <param name="builder">The web application builder to configure.</param>
    /// <returns>The configured web application builder for method chaining.</returns>
    /// <remarks>
    /// This method registers an exception handler and enables problem details responses
    /// for standardized error handling across the application.
    /// Added in version 1.0.0.
    /// </remarks>
    public static WebApplicationBuilder AddCustomProblemDetails<TExceptionHandler>(this WebApplicationBuilder builder)
        where TExceptionHandler : class, IExceptionHandler
    {
        builder.Services.AddExceptionHandler<TExceptionHandler>();
        builder.Services.AddProblemDetails();

        return builder;
    }
}
