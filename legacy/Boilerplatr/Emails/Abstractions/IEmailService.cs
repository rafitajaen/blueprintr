using Boilerplatr.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MimeKit;

namespace Boilerplatr.Emails.Abstractions;

public interface IEmailService<TContext> : IHostedService where TContext : DbContext
{
    Task<bool> TryEnqueueAsync(DbContext dbContext, IEnumerable<MimeMessage> messages, Guid7? tenantId = null, CancellationToken cancellationToken = default);
}
