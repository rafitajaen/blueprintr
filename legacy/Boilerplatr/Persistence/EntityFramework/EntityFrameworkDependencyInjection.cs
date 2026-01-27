using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Boilerplatr.Persistence.EntityFramework;

public static class EntityFrameworkDependencyInjection
{
    public static DbContextOptionsBuilder UseDefaultNpgsql(this DbContextOptionsBuilder options, string connectionString, IConfiguration? configuration = null)
    {
        return options.UseNpgsql
        (
            connectionString: connectionString,
            npgsqlOptionsAction: o => o.UseNodaTime().UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery)
        )
        .UseSnakeCaseNamingConvention()
        .EnableThreadSafetyChecks(false);
    }

    public static WebApplicationBuilder AddModule<TContext>(this WebApplicationBuilder builder, string? connectionString) where TContext : DbContext
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString, nameof(connectionString));

        builder.Services.AddDbContextPool<TContext>
        (
            // contextLifetime: ServiceLifetime.Scoped,
            optionsAction: o => o.UseNpgsql
            (
                connectionString: connectionString,
                npgsqlOptionsAction: o => o.UseNodaTime().UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery)
            )
            .UseSnakeCaseNamingConvention()
            .EnableThreadSafetyChecks(false)
            // .MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Default)
        );

        return builder;
    }

    public static IApplicationBuilder UseModule<TContext>(this WebApplication app) where TContext : DbContext
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
