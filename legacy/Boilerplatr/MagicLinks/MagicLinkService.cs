using Boilerplatr.Emails;
using Boilerplatr.Emails.Abstractions;
using Boilerplatr.Extensions;
using Boilerplatr.Shared;
using Boilerplatr.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using NodaTime;

namespace Boilerplatr.MagicLinks;

public interface IMagicLinkService<TContext> : IHostedService where TContext : DbContext
{
    Task<bool> TrySendAsync(TContext db, BaseEmailTemplateModel template, string userId, string email, string? returnUrl = null, Guid7? tenantId = null, CancellationToken cancellationToken = default);
}

public sealed class MagicLinkService<TContext>
(
    IServiceScopeFactory factory,
    IOptions<MagicLinkOptions> magicOptions,
    MagicLinkTemplate renderer,
    IEmailService<TContext> emailService,
    ILogger<MagicLinkService<TContext>> logger
) : BackgroundService, IMagicLinkService<TContext> where TContext : DbContext
{

    private readonly TimeSpan LoopDelay = TimeSpan.FromSeconds(30);
    private string ServiceName => typeof(MagicLinkService<TContext>).FullName ?? nameof(MagicLinkService<TContext>);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogStartingService(ServiceName);

        await Task.Delay(LoopDelay, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = SystemClock.Instance.GetCurrentInstant();

            try
            {
                await using var scope = factory.CreateAsyncScope();
                using var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();

                await dbContext.Set<MagicLink>()
                        .Where(link => link.ExpiredAt <= now || link.HasBeenUsed)
                        .ExecuteDeleteAsync(stoppingToken);

            }
            catch (Exception ex)
            {
                logger.LogCriticalUnexpectedException
                (
                    serviceName: ServiceName,
                    methodName: nameof(ExecuteAsync),
                    exceptionMessage: ex.Message
                );
            }
            finally
            {
                await Task.Delay(LoopDelay, stoppingToken);
            }
        }
    }

    public async Task<bool> TrySendAsync(TContext db, BaseEmailTemplateModel template, string userId, string email, string? returnUrl = null, Guid7? tenantId = null, CancellationToken cancellationToken = default)
    {
        var userGuid = Guid7.Parse(userId);

        await db.Set<MagicLink>()
            .Where(x => x.UserId == userGuid)
            .ExecuteDeleteAsync(cancellationToken);

        var now = SystemClock.Instance.GetCurrentInstant();
        var link = new MagicLink()
        {
            Id = Ulid.NewUlid(),
            UserId = userGuid,
            Email = email,
            ReturnUrl = returnUrl,
            CreatedAt = now,
            ExpiredAt = now.Plus(Duration.FromMinutes(magicOptions.Value.ExpirationMinutes)),
        };

        await db.Set<MagicLink>().AddAsync(link, cancellationToken);

        var model = new MagicLinkTemplateModel
        (
            link: $"{template.ProjectUrl}/{magicOptions.Value.BaseUrl}?code={link.Id}".NormalizeUrl(),
            model: template
        );

        try
        {
            var mime = new MimeMessage();
            mime.From.Add(new MailboxAddress(model.FromName, model.FromEmail));
            mime.To.Add(new MailboxAddress(email, email));
            mime.Subject = model.Subject;
            mime.Body = new BodyBuilder() { HtmlBody = renderer.RenderHtmlEmail(model) }.ToMessageBody();

            return await emailService.TryEnqueueAsync(db, [mime], tenantId, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogErrorUnexpectedException
            (
                serviceName: nameof(MagicLinkService<TContext>),
                methodName: nameof(TrySendAsync),
                exceptionMessage: ex.Message
            );
        }

        return false;
    }
}
