using Boilerplatr.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Boilerplatr.Sitemaps;

public interface ISitemapService<TContext> : IHostedService where TContext : DbContext
{
    Task<bool> TryGenerateAsync(DbContext dbContext, Tenant tenant, CancellationToken cancellationToken = default);
}
