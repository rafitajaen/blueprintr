using Boilerplatr.Abstractions.Entities;
using Boilerplatr.Shared;
using Boilerplatr.Tenants;
using MimeKit;
using NodaTime;

namespace Boilerplatr.Emails;

public sealed class Email : Entity<Guid7>
{
    public override Guid7 Id { get; init; }

    public required EmailMessage Message { get; set; }

    public string? Response { get; set; }

    public required Instant CreatedAt { get; set; }
    public Instant? SentAt { get; set; }

    public Instant? SendSince { get; set; }
    public Instant? SendUntil { get; set; }

    public int Fails { get; set; }
    public Instant? LastFailAt { get; set; }

    // Relations
    public Guid7? TenantId { get; set; }
    public Tenant? Tenant { get; set; }
}

public sealed class EmailMessage
{
    public string? FromName { get; set; }
    public string FromAddress { get; set; }
    public string? ToName { get; set; }
    public string ToAddress { get; set; }
    public string? Subject { get; set; }
    public string? TextBody { get; set; }
    public string? HtmlBody { get; set; }

    public bool IsEmpty() => string.IsNullOrWhiteSpace(TextBody) && string.IsNullOrWhiteSpace(HtmlBody);

    public MimeMessage ToMime()
    {
        var message = new MimeMessage();

        if (!string.IsNullOrWhiteSpace(FromAddress))
        {
            var from = new MailboxAddress
            (
                name: string.IsNullOrWhiteSpace(FromName) ? FromAddress : FromName,
                address: FromAddress
            );

            message.From.Add(from);
        }

        if (!string.IsNullOrWhiteSpace(ToAddress))
        {
            var to = new MailboxAddress
            (
                name: string.IsNullOrWhiteSpace(ToName) ? ToAddress : ToName,
                address: ToAddress
            );

            message.To.Add(to);
        }

        if (!string.IsNullOrWhiteSpace(Subject))
        {
            message.Subject = Subject;
        }

        if (!string.IsNullOrWhiteSpace(TextBody) || !string.IsNullOrWhiteSpace(HtmlBody))
        {
            var bb = new BodyBuilder();

            if (!string.IsNullOrWhiteSpace(TextBody))
            {
                bb.TextBody = TextBody;
            }

            if (!string.IsNullOrWhiteSpace(HtmlBody))
            {
                bb.HtmlBody = HtmlBody;
            }

            message.Body = bb.ToMessageBody();
        }

        return message;
    }

    public static EmailMessage From(MimeMessage mime)
    {
        return new()
        {
            FromName = mime.From.Mailboxes.FirstOrDefault()?.Name,
            FromAddress = mime.From.Mailboxes.FirstOrDefault()?.Address ?? string.Empty,
            ToName = mime.To.Mailboxes.FirstOrDefault()?.Name,
            ToAddress = mime.To.Mailboxes.FirstOrDefault()?.Address ?? string.Empty,
            Subject = mime.Subject,
            TextBody = mime.TextBody,
            HtmlBody = mime.HtmlBody,
        };
    }

}
