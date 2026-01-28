using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Blueprintr.EntityFramework;

/// <summary>
/// Provides extension methods for configuring Entity Framework Core with PostgreSQL and automatic migrations.
/// </summary>
/// <remarks>
/// This class simplifies the setup of Entity Framework Core DbContexts with opinionated defaults
/// optimized for PostgreSQL, including connection pooling, snake_case naming conventions, and
/// automatic migrations with optional seeding in development environments.
/// </remarks>
public static class EntityFrameworkDependencyInjection
{
    /// <summary>
    /// Registers a DbContext with optimized PostgreSQL configuration and connection pooling.
    /// </summary>
    /// <typeparam name="TContext">The type of the DbContext to register.</typeparam>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> to configure.</param>
    /// <param name="connectionString">The PostgreSQL connection string.</param>
    /// <returns>The <see cref="WebApplicationBuilder"/> for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="connectionString"/> is null, empty, or whitespace.</exception>
    /// <remarks>
    /// <para>This method configures the DbContext with the following optimizations:</para>
    /// <list type="bullet">
    /// <item><b>Connection pooling:</b> Uses <c>AddDbContextPool</c> for improved performance by reusing context instances.</item>
    /// <item><b>PostgreSQL provider:</b> Configured with Npgsql for PostgreSQL support.</item>
    /// <item><b>NodaTime support:</b> Enables modern date/time handling with NodaTime types.</item>
    /// <item><b>Single query splitting:</b> Optimizes LINQ queries to use single queries instead of split queries.</item>
    /// <item><b>Snake case naming:</b> Automatically converts C# PascalCase names to database snake_case (e.g., UserName → user_name).</item>
    /// <item><b>Thread safety disabled:</b> Improves performance in ASP.NET Core where contexts are scoped per request.</item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// var builder = WebApplication.CreateBuilder(args);
    ///
    /// // Configure DbContext with PostgreSQL
    /// builder.AddDbContext&lt;AppDbContext&gt;(
    ///     builder.Configuration.GetConnectionString("DefaultConnection")
    /// );
    ///
    /// var app = builder.Build();
    /// </code>
    /// </example>
    /// <seealso cref="UseDbContext{TContext}"/>
    public static WebApplicationBuilder AddDbContext<TContext>(this WebApplicationBuilder builder, string? connectionString) where TContext : DbContext
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString, nameof(connectionString));

        builder.Services.AddDbContextPool<TContext>
        (
            optionsAction: o => o.UseNpgsql
            (
                connectionString: connectionString,
                npgsqlOptionsAction: o => o.UseNodaTime().UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery)
            )
            .UseSnakeCaseNamingConvention()
            .EnableThreadSafetyChecks(false)
        );

        return builder;
    }

    /// <summary>
    /// Applies pending migrations and optionally seeds the database in development environments.
    /// </summary>
    /// <typeparam name="TContext">The type of the DbContext to initialize.</typeparam>
    /// <param name="app">The <see cref="WebApplication"/> to configure.</param>
    /// <returns>The <see cref="IApplicationBuilder"/> for method chaining.</returns>
    /// <remarks>
    /// <para>This method performs the following operations during application startup:</para>
    /// <list type="number">
    /// <item><b>Apply migrations:</b> Automatically runs <c>Database.Migrate()</c> to apply pending migrations.</item>
    /// <item><b>Initialize database:</b> If the context implements <see cref="ISeedeableDbContext"/>, calls <see cref="ISeedeableDbContext.Initialize"/>.</item>
    /// <item><b>Seed data (Development only):</b> In development environments, checks <see cref="ISeedeableDbContext.IsSeeded"/>
    /// and calls <see cref="ISeedeableDbContext.Seed"/> if the database is empty.</item>
    /// <item><b>Save changes:</b> Automatically saves seeded data to the database.</item>
    /// </list>
    /// <para><b>⚠️ Production Warning:</b> Automatic migrations run in all environments. Consider using
    /// manual migration deployment strategies in production to avoid downtime and enable rollback capabilities.</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var builder = WebApplication.CreateBuilder(args);
    ///
    /// builder.AddDbContext&lt;AppDbContext&gt;(
    ///     builder.Configuration.GetConnectionString("DefaultConnection")
    /// );
    ///
    /// var app = builder.Build();
    ///
    /// // Apply migrations and seed database
    /// app.UseDbContext&lt;AppDbContext&gt;();
    ///
    /// app.Run();
    /// </code>
    /// </example>
    /// <seealso cref="AddDbContext{TContext}"/>
    /// <seealso cref="ISeedeableDbContext"/>
    public static IApplicationBuilder UseDbContext<TContext>(this WebApplication app) where TContext : DbContext
    {
        using (var scope = app.Services.CreateScope())
        {
            using var db = scope.ServiceProvider.GetRequiredService<TContext>();

            db.Database.Migrate();

            if (db is ISeedeableDbContext context)
            {
                context.Initialize();

                if (app.Environment.IsDevelopment() && !context.IsSeeded())
                {
                    context.Seed();

                    db.SaveChanges();
                }
            }
        }

        return app;
    }
}
