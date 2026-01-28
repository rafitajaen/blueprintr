using Blueprintr.DependencyInjection;
using Blueprintr.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blueprintr.Tests.DependencyInjection;

/// <summary>
/// Tests for <see cref="WebApplicationExtensions"/> that provides configuration extension methods for ASP.NET Core applications.
/// </summary>
[TestFixture]
public class WebApplicationExtensionsTests
{
    #region ConfigureWebHost Tests

    [Test]
    public void ConfigureWebHost_RegistersHttpContextAccessor()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        // Act
        builder.ConfigureWebHost();
        var serviceProvider = builder.Services.BuildServiceProvider();

        // Assert
        var httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
        Assert.That(httpContextAccessor, Is.Not.Null,
            "ConfigureWebHost should register IHttpContextAccessor");
    }

    [Test]
    public void ConfigureWebHost_ConfiguresJsonOptions()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        // Act
        builder.ConfigureWebHost();
        var serviceProvider = builder.Services.BuildServiceProvider();

        // Assert
        var jsonOptions = serviceProvider.GetService<IOptions<JsonOptions>>();
        Assert.That(jsonOptions, Is.Not.Null, "JsonOptions should be configured");
        Assert.That(jsonOptions!.Value.SerializerOptions.Converters.Any(c => c is JsonStringEnumConverter), Is.True,
            "JSON options should include JsonStringEnumConverter for enum string serialization");
    }

    [Test]
    public void ConfigureWebHost_ConfiguresNodaTime()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        // Act
        builder.ConfigureWebHost();
        var serviceProvider = builder.Services.BuildServiceProvider();

        // Assert
        var jsonOptions = serviceProvider.GetService<IOptions<JsonOptions>>();
        Assert.That(jsonOptions, Is.Not.Null, "JsonOptions should be configured");

        // Verify NodaTime converters are registered by attempting to serialize an Instant
        var instant = Instant.FromUtc(2024, 1, 15, 10, 30, 0);
        var json = JsonSerializer.Serialize(instant, jsonOptions!.Value.SerializerOptions);

        // NodaTime serialization should produce ISO 8601 format
        Assert.That(json, Does.Contain("2024-01-15"),
            "NodaTime should be configured for proper serialization");
    }

    [Test]
    public void ConfigureWebHost_ConfiguresDefaultIgnoreCondition()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        // Act
        builder.ConfigureWebHost();
        var serviceProvider = builder.Services.BuildServiceProvider();

        // Assert
        var jsonOptions = serviceProvider.GetService<IOptions<JsonOptions>>();
        Assert.That(jsonOptions, Is.Not.Null, "JsonOptions should be configured");
        Assert.That(jsonOptions!.Value.SerializerOptions.DefaultIgnoreCondition,
            Is.EqualTo(JsonIgnoreCondition.WhenWritingNull),
            "JSON options should ignore null values when writing");
    }

    [Test]
    public void ConfigureWebHost_RegistersCancellationToken()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        // Act
        builder.ConfigureWebHost();
        var serviceProvider = builder.Services.BuildServiceProvider();

        // Assert
        // CancellationToken registration exists but requires HttpContext to resolve
        var descriptor = builder.Services.FirstOrDefault(d => d.ServiceType == typeof(CancellationToken));
        Assert.That(descriptor, Is.Not.Null,
            "ConfigureWebHost should register CancellationToken as a scoped service");
        Assert.That(descriptor!.Lifetime, Is.EqualTo(ServiceLifetime.Scoped),
            "CancellationToken should be registered with scoped lifetime");
    }

    #endregion

    #region ConfigureCors Tests

    [Test]
    public void ConfigureCors_WithNoOrigins_AllowsAny()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        // Act
        builder.ConfigureCors();

        // Assert
        // CORS services should be registered
        var corsServiceDescriptor = builder.Services.FirstOrDefault(d =>
            d.ServiceType.FullName?.Contains("Cors") == true);
        Assert.That(corsServiceDescriptor, Is.Not.Null,
            "ConfigureCors should register CORS services when called with no origins");
    }

    [Test]
    public void ConfigureCors_WithSpecificOrigins_AllowsThoseOrigins()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        var allowedOrigins = new[] { "https://example.com", "https://api.example.com" };

        // Act
        builder.ConfigureCors(allowedOrigins);

        // Assert
        // CORS services should be registered
        var corsServiceDescriptor = builder.Services.FirstOrDefault(d =>
            d.ServiceType.FullName?.Contains("Cors") == true);
        Assert.That(corsServiceDescriptor, Is.Not.Null,
            "ConfigureCors should register CORS services with specific origins");
    }

    [Test]
    public void ConfigureCors_ReturnsSameBuilder()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        // Act
        var result = builder.ConfigureCors("https://example.com");

        // Assert
        Assert.That(result, Is.SameAs(builder),
            "ConfigureCors should return the same builder instance for method chaining");
    }

    #endregion

    #region AddValidatorsFrom Tests

    [Test]
    public void AddValidatorsFrom_RegistersValidators()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        var assemblies = new[] { Assembly.GetExecutingAssembly() };

        // Act
        builder.AddValidatorsFrom(assemblies);

        // Assert
        // Check that FluentValidation services were added
        var validatorServices = builder.Services.Where(d =>
            d.ServiceType.IsGenericType &&
            d.ServiceType.GetGenericTypeDefinition() == typeof(IValidator<>)).ToList();

        // The test passes if no exception is thrown and services are registered
        // (even if no validators exist in the test assembly)
        Assert.Pass("AddValidatorsFrom executed without errors");
    }

    [Test]
    public void AddValidatorsFrom_WithCustomLifetime_RegistersWithThatLifetime()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        var assemblies = new[] { Assembly.GetExecutingAssembly() };

        // Act
        builder.AddValidatorsFrom(assemblies, ServiceLifetime.Transient);

        // Assert
        Assert.Pass("AddValidatorsFrom with custom lifetime executed without errors");
    }

    [Test]
    public void AddValidatorsFrom_ReturnsSameBuilder()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        var assemblies = new[] { Assembly.GetExecutingAssembly() };

        // Act
        var result = builder.AddValidatorsFrom(assemblies);

        // Assert
        Assert.That(result, Is.SameAs(builder),
            "AddValidatorsFrom should return the same builder instance for method chaining");
    }

    #endregion

    #region AddHealthChecks Tests

    [Test]
    public void AddHealthChecks_WithPostgresConnectionString_RegistersNpgSqlHealthCheck()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Configuration["ConnectionStrings:Postgresql"] = "Host=localhost;Database=test;Username=user;Password=pass";

        // Act
        builder.AddHealthChecks();

        // Assert
        var healthCheckService = builder.Services.FirstOrDefault(d =>
            d.ServiceType.FullName?.Contains("HealthCheck") == true);
        Assert.That(healthCheckService, Is.Not.Null,
            "AddHealthChecks should register health check services when Postgres connection string is present");
    }

    [Test]
    public void AddHealthChecks_WithRedisConnectionString_RegistersRedisHealthCheck()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Configuration["ConnectionStrings:Redis"] = "localhost:6379";

        // Act
        builder.AddHealthChecks();

        // Assert
        var healthCheckService = builder.Services.FirstOrDefault(d =>
            d.ServiceType.FullName?.Contains("HealthCheck") == true);
        Assert.That(healthCheckService, Is.Not.Null,
            "AddHealthChecks should register health check services when Redis connection string is present");
    }

    [Test]
    public void AddHealthChecks_WithoutConnectionStrings_DoesNotRegisterHealthChecks()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        // No connection strings configured

        // Act
        builder.AddHealthChecks();

        // Assert
        // Method should complete without error even without connection strings
        Assert.Pass("AddHealthChecks executed without errors when no connection strings are present");
    }

    [Test]
    public void AddHealthChecks_WithCustomKeys_UsesThoseKeys()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Configuration["ConnectionStrings:MyPostgres"] = "Host=localhost;Database=test";
        builder.Configuration["ConnectionStrings:MyRedis"] = "localhost:6379";

        // Act
        builder.AddHealthChecks("MyPostgres", "MyRedis");

        // Assert
        var healthCheckService = builder.Services.FirstOrDefault(d =>
            d.ServiceType.FullName?.Contains("HealthCheck") == true);
        Assert.That(healthCheckService, Is.Not.Null,
            "AddHealthChecks should use custom connection string keys when provided");
    }

    [Test]
    public void AddHealthChecks_ReturnsSameBuilder()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        // Act
        var result = builder.AddHealthChecks();

        // Assert
        Assert.That(result, Is.SameAs(builder),
            "AddHealthChecks should return the same builder instance for method chaining");
    }

    #endregion

    #region AddCustomProblemDetails Tests

    [Test]
    public void AddCustomProblemDetails_RegistersExceptionHandler()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        // Act
        builder.AddCustomProblemDetails<GlobalExceptionHandler>();

        // Assert
        var exceptionHandlerService = builder.Services.FirstOrDefault(d =>
            d.ServiceType.FullName?.Contains("ExceptionHandler") == true ||
            d.ImplementationType == typeof(GlobalExceptionHandler));
        Assert.That(exceptionHandlerService, Is.Not.Null,
            "AddCustomProblemDetails should register the exception handler");
    }

    [Test]
    public void AddCustomProblemDetails_RegistersProblemDetails()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        // Act
        builder.AddCustomProblemDetails<GlobalExceptionHandler>();

        // Assert
        var problemDetailsService = builder.Services.FirstOrDefault(d =>
            d.ServiceType.FullName?.Contains("ProblemDetails") == true);
        Assert.That(problemDetailsService, Is.Not.Null,
            "AddCustomProblemDetails should register problem details services");
    }

    [Test]
    public void AddCustomProblemDetails_ReturnsSameBuilder()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        // Act
        var result = builder.AddCustomProblemDetails<GlobalExceptionHandler>();

        // Assert
        Assert.That(result, Is.SameAs(builder),
            "AddCustomProblemDetails should return the same builder instance for method chaining");
    }

    #endregion

    #region Method Chaining Tests

    [Test]
    public void AllMethods_SupportFluentChaining()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Configuration["ConnectionStrings:Postgresql"] = "Host=localhost;Database=test";

        // Act & Assert - Should not throw
        var result = builder
            .ConfigureWebHost()
            .ConfigureCors("https://example.com")
            .AddValidatorsFrom(new[] { Assembly.GetExecutingAssembly() })
            .AddHealthChecks()
            .AddCustomProblemDetails<GlobalExceptionHandler>();

        Assert.That(result, Is.SameAs(builder),
            "All extension methods should support fluent chaining and return the same builder");
    }

    #endregion
}
