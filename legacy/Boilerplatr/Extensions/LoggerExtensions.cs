using Microsoft.Extensions.Logging;

namespace Boilerplatr.Extensions;

public static partial class LoggerExtensions
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Starting service `{ServiceName}`...")]
    public static partial void LogStartingService(this ILogger logger, string serviceName);

    [LoggerMessage(Level = LogLevel.Information, Message = "Unexpected Exception in {ServiceName}::{MethodName}. Exception Message: {ExceptionMessage}.")]
    public static partial void LogInformationUnexpectedException(this ILogger logger, string serviceName, string methodName, string exceptionMessage);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Unexpected Exception in {ServiceName}::{MethodName}. Exception Message: {ExceptionMessage}.")]
    public static partial void LogWarningUnexpectedException(this ILogger logger, string serviceName, string methodName, string exceptionMessage);

    [LoggerMessage(Level = LogLevel.Error, Message = "Unexpected Exception in {ServiceName}::{MethodName}. Exception Message: {ExceptionMessage}.")]
    public static partial void LogErrorUnexpectedException(this ILogger logger, string serviceName, string methodName, string exceptionMessage);

    [LoggerMessage(Level = LogLevel.Critical, Message = "Unexpected Exception in {ServiceName}::{MethodName}. Exception Message: {ExceptionMessage}.")]
    public static partial void LogCriticalUnexpectedException(this ILogger logger, string serviceName, string methodName, string exceptionMessage);
}