using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Blueprintr.Exceptions;

/// <summary>
/// Global exception handler that catches unhandled exceptions and returns standardized problem details responses.
/// </summary>
/// <param name="logger">Logger for recording exception information.</param>
/// <remarks>
/// This handler implements <see cref="IExceptionHandler"/> to provide centralized exception handling
/// across the application. All unhandled exceptions are logged and converted to RFC 7231 compliant
/// problem details responses with a 500 Internal Server Error status.
/// Added in version 1.0.0.
/// </remarks>
public sealed class GlobalExceptionHandler
(
    ILogger<GlobalExceptionHandler> logger
) : IExceptionHandler
{
    /// <summary>
    /// Attempts to handle the specified exception by logging it and returning a problem details response.
    /// </summary>
    /// <param name="httpContext">The HTTP context for the current request.</param>
    /// <param name="exception">The exception that occurred.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is always true,
    /// indicating the exception was handled.
    /// </returns>
    /// <remarks>
    /// This method logs the exception as an error and returns a standardized problem details response
    /// with status code 500 (Internal Server Error). The response follows RFC 7231 specifications.
    /// Added in version 1.0.0.
    /// </remarks>
    public async ValueTask<bool> TryHandleAsync
    (
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        logger.LogError(exception, "Unhandled exception occurred");

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
            Title = "Server failure"
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
