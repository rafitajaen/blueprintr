namespace Blueprintr.EntityFramework;

/// <summary>
/// Defines a contract for database contexts that support automatic seeding and initialization.
/// </summary>
/// <remarks>
/// Implement this interface on your Entity Framework DbContext to enable automatic seeding
/// in development environments. The <see cref="EntityFrameworkDependencyInjection.UseDbContext{TContext}"/>
/// method will detect this interface and call the appropriate methods during application startup.
/// </remarks>
/// <example>
/// <code>
/// public class AppDbContext : DbContext, ISeedeableDbContext
/// {
///     public DbSet&lt;User&gt; Users { get; set; }
///
///     public void Initialize()
///     {
///         // Configure initial database state
///     }
///
///     public bool IsSeeded()
///     {
///         return Users.Any();
///     }
///
///     public void Seed()
///     {
///         Users.AddRange(
///             new User { Name = "Admin" },
///             new User { Name = "Test User" }
///         );
///     }
/// }
/// </code>
/// </example>
public interface ISeedeableDbContext
{
    /// <summary>
    /// Seeds the database with initial test or development data.
    /// </summary>
    /// <remarks>
    /// This method is called automatically in development environments when <see cref="IsSeeded"/>
    /// returns <c>false</c>. Changes are automatically saved after this method executes.
    /// </remarks>
    void Seed();

    /// <summary>
    /// Determines whether the database has already been seeded.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the database contains seed data; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method is called to avoid re-seeding data on every application startup.
    /// Typically checks if key tables contain data.
    /// </remarks>
    bool IsSeeded();

    /// <summary>
    /// Performs custom initialization logic before seeding.
    /// </summary>
    /// <remarks>
    /// This method is called before checking if seeding is needed. Use it for:
    /// <list type="bullet">
    /// <item>Creating views or stored procedures</item>
    /// <item>Configuring database-specific settings</item>
    /// <item>Setting up audit triggers</item>
    /// <item>Any other initialization that should run on every startup</item>
    /// </list>
    /// </remarks>
    void Initialize();
}
