using Microsoft.Extensions.Logging;

namespace Boilerplatr.Tenants;

public static partial class LoggerExtensions
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Fordwarded Host: {Host}")]
    public static partial void LogForwardedHost(this ILogger logger, string host);
}
