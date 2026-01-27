using Microsoft.Extensions.Logging;

namespace Boilerplatr.Emails;

public static partial class LoggerExtensions
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Email [{EmailId}] was sent to {EmailAddress}. Subject: {EmailSubject}")]
    public static partial void LogEmailWasSent(this ILogger logger, string emailId, string emailAddress, string emailSubject);
}
