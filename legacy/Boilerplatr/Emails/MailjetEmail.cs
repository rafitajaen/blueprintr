namespace Boilerplatr.Emails;

public class MailjetEmail
{
    public MailjetAddress From { get; set; }
    public IEnumerable<MailjetAddress> To { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string TextPart { get; set; } = string.Empty;
    public string HTMLPart { get; set; } = string.Empty;
    public bool SandboxMode { get; set; }

    public MailjetEmail
    (
        string fromName,
        string fromEmail,
        string toName,
        string toEmail,
        string subject,
        string text,
        string html
    )
    {
        From = new(fromName, fromEmail);
        To = [new(toName, toEmail)];
        Subject = subject;
        TextPart = text;
        HTMLPart = html;
    }

    public MailjetEmail(Email email)
    {
        From = new(email.Message.FromName ?? email.Message.FromAddress, email.Message.FromAddress);
        To = [new(email.Message.ToName ?? email.Message.ToAddress, email.Message.ToAddress)];
        Subject = email.Message.Subject ?? string.Empty;
        TextPart = email.Message.TextBody ?? string.Empty;
        HTMLPart = email.Message.HtmlBody ?? string.Empty;
    }
}

public class MailjetAddress
{
    public string Name { get; set; }
    public string Email { get; set; }

    public MailjetAddress(string name, string email)
    {
        Name = name;
        Email = email;
    }
}
