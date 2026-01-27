using MimeKit;

namespace Boilerplatr.Extensions;

public static class MimeExtensions
{
    public static bool HasEmptyBody(this MimeMessage message)
    {
        return string.IsNullOrWhiteSpace(message.TextBody) && string.IsNullOrWhiteSpace(message.HtmlBody);
    }
}
