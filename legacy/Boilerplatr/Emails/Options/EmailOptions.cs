using Boilerplatr.Options;

namespace Boilerplatr.Emails.Options;

public class EmailOptions : ICustomOptions<EmailOptions>
{
    public string? SmtpHost { get; set; }
    public int? SmtpPort { get; set; }

    public string? Username { get; set; }
    public string? Password { get; set; }

    public string? ApiKey { get; set; }
    public string? SecretKey { get; set; }

    public bool IsSmtp() => !string.IsNullOrWhiteSpace(SmtpHost) && SmtpPort > 0;
    public bool HasAuthentication() => !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
    public bool HasApiKeys() => !string.IsNullOrWhiteSpace(ApiKey) && !string.IsNullOrWhiteSpace(SecretKey);
}
