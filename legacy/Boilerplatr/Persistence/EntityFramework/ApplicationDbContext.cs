using Boilerplatr.Abstractions.DomainEvents;
using Boilerplatr.Abstractions.Entities;
using Microsoft.EntityFrameworkCore;

namespace Boilerplatr.Persistence.EntityFramework;

public abstract class ApplicationDbContext<TContext> : DbContext, IUnitOfWork where TContext : DbContext
{
    // ! Mandatory: 
    // * Entity Framework Migrations needs an empty constructor because there are multiple `DbContexts` in the same project.
    // ? More info: https://learn.microsoft.com/en-us/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli
    public ApplicationDbContext() : base(new DbContextOptions<TContext>()) { }
    public ApplicationDbContext
    (
        DbContextOptions<TContext> options,
        // IConfiguration configuration,
        IDomainEventsDispatcher? domainEventsDispatcher = default
    ) : base(options)
    {
        // _configuration = configuration;
        _domainEventsDispatcher = domainEventsDispatcher;
    }

    // private readonly IConfiguration _configuration;
    private readonly IDomainEventsDispatcher? _domainEventsDispatcher;

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     base.OnConfiguring(optionsBuilder);
    //     optionsBuilder.UseDefaultNpgsql(_configuration);
    // }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // When should you publish domain events?
        //
        // 1. BEFORE calling SaveChangesAsync
        //     - domain events are part of the same transaction
        //     - immediate consistency
        // 2. AFTER calling SaveChangesAsync
        //     - domain events are a separate transaction
        //     - eventual consistency
        //     - handlers can fail

        await PublishDomainEventsAsync();
        int result = await base.SaveChangesAsync(cancellationToken);

        return result;
    }

    private async Task PublishDomainEventsAsync()
    {
        if (_domainEventsDispatcher is not null)
        {
            var domainEvents = ChangeTracker
                .Entries<IEntity>()
                .Select(entry => entry.Entity)
                .SelectMany(entity =>
                {
                    var domainEvents = entity.GetEvents();
                    entity.ClearEvents();

                    return domainEvents;
                })
                .ToList();

            await _domainEventsDispatcher.DispatchAsync(domainEvents);
        }
    }

}
