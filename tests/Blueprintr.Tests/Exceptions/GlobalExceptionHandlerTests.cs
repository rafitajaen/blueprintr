using Blueprintr.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Text.Json;

namespace Blueprintr.Tests.Exceptions;

/// <summary>
/// Tests for <see cref="GlobalExceptionHandler"/> exception handling middleware.
/// </summary>
[TestFixture]
public class GlobalExceptionHandlerTests
{
    private ILogger<GlobalExceptionHandler> _logger = null!;
    private GlobalExceptionHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _logger = Substitute.For<ILogger<GlobalExceptionHandler>>();
        _handler = new GlobalExceptionHandler(_logger);
    }

    [Test]
    public async Task TryHandleAsync_WithException_ReturnsTrue()
    {
        // Arrange
        var httpContext = CreateHttpContext();
        var exception = new InvalidOperationException("Test exception");
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _handler.TryHandleAsync(httpContext, exception, cancellationToken);

        // Assert
        Assert.That(result, Is.True, "Handler should always return true to indicate exception was handled");
    }

    [Test]
    public async Task TryHandleAsync_SetsStatusCodeTo500()
    {
        // Arrange
        var httpContext = CreateHttpContext();
        var exception = new Exception("Test exception");
        var cancellationToken = CancellationToken.None;

        // Act
        await _handler.TryHandleAsync(httpContext, exception, cancellationToken);

        // Assert
        Assert.That(httpContext.Response.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
    }

    [Test]
    public async Task TryHandleAsync_WritesProblemDetailsToResponse()
    {
        // Arrange
        var httpContext = CreateHttpContext();
        var exception = new Exception("Test exception");
        var cancellationToken = CancellationToken.None;

        // Act
        await _handler.TryHandleAsync(httpContext, exception, cancellationToken);

        // Assert
        httpContext.Response.Body.Position = 0;
        var responseBody = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();

        Assert.That(responseBody, Is.Not.Empty);

        var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.That(problemDetails, Is.Not.Null);
        Assert.That(problemDetails!.Status, Is.EqualTo(StatusCodes.Status500InternalServerError));
        Assert.That(problemDetails.Type, Is.EqualTo("https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"));
        Assert.That(problemDetails.Title, Is.EqualTo("Server failure"));
    }

    [Test]
    public async Task TryHandleAsync_LogsError()
    {
        // Arrange
        var httpContext = CreateHttpContext();
        var exception = new InvalidOperationException("Critical error occurred");
        var cancellationToken = CancellationToken.None;

        // Act
        await _handler.TryHandleAsync(httpContext, exception, cancellationToken);

        // Assert
        _logger.Received(1).LogError(exception, "Unhandled exception occurred");
    }

    [Test]
    public async Task TryHandleAsync_HandlesCancellationToken()
    {
        // Arrange
        var httpContext = CreateHttpContext();
        var exception = new Exception("Test exception");
        var cancellationTokenSource = new CancellationTokenSource();

        // Act - Start handling but cancel immediately
        cancellationTokenSource.Cancel();

        // Assert - Should throw TaskCanceledException (derived from OperationCanceledException) when trying to write to response
        Assert.ThrowsAsync<TaskCanceledException>(async () =>
            await _handler.TryHandleAsync(httpContext, exception, cancellationTokenSource.Token));
    }

    [Test]
    public async Task TryHandleAsync_WithDifferentExceptionTypes_AlwaysReturnsTrue()
    {
        // Arrange
        var httpContext1 = CreateHttpContext();
        var httpContext2 = CreateHttpContext();
        var httpContext3 = CreateHttpContext();

        var exception1 = new InvalidOperationException("Invalid operation");
        var exception2 = new ArgumentNullException("param", "Parameter is null");
        var exception3 = new NotImplementedException("Not implemented yet");

        var cancellationToken = CancellationToken.None;

        // Act
        var result1 = await _handler.TryHandleAsync(httpContext1, exception1, cancellationToken);
        var result2 = await _handler.TryHandleAsync(httpContext2, exception2, cancellationToken);
        var result3 = await _handler.TryHandleAsync(httpContext3, exception3, cancellationToken);

        // Assert
        Assert.That(result1, Is.True);
        Assert.That(result2, Is.True);
        Assert.That(result3, Is.True);
    }

    [Test]
    public async Task TryHandleAsync_WithNullException_StillHandlesGracefully()
    {
        // Arrange
        var httpContext = CreateHttpContext();
        Exception? exception = null;
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _handler.TryHandleAsync(httpContext, exception!, cancellationToken);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(httpContext.Response.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
    }

    [Test]
    public async Task TryHandleAsync_ResponseContentTypeIsApplicationJson()
    {
        // Arrange
        var httpContext = CreateHttpContext();
        var exception = new Exception("Test exception");
        var cancellationToken = CancellationToken.None;

        // Act
        await _handler.TryHandleAsync(httpContext, exception, cancellationToken);

        // Assert
        Assert.That(httpContext.Response.ContentType, Does.Contain("application/json"));
    }

    // Helper methods

    private static HttpContext CreateHttpContext()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();
        return httpContext;
    }
}
