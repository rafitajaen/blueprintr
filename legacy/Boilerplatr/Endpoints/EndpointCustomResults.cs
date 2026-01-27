using Boilerplatr.Abstractions;
using Boilerplatr.Abstractions.Errors;
using Boilerplatr.Abstractions.Results;
using Microsoft.AspNetCore.Http;

namespace Boilerplatr.Endpoints;

public static class EndpointCustomResults
{
    public static IResult Problem(Result result) => result.IsSuccess
        ? throw new InvalidOperationException()
        : Results.Problem
        (
            title: result.Error.Code,
            detail: result.Error.Description,
            type: GetType(result.Error.Type),
            statusCode: GetStatusCode(result.Error.Type),
            extensions: GetErrors(result)
        );

    private static string GetType(ErrorType errorType) => errorType switch
    {
        ErrorType.Validation => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
        ErrorType.Problem => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
        ErrorType.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
        ErrorType.Unauthorized => "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1",
        ErrorType.Forbidden => "https://tools.ietf.org/html/rfc7231#section-6.5.3",
        ErrorType.Conflict => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
        _ => "https://tools.ietf.org/html/rfc7231#section-6.6.1"
    };

    private static int GetStatusCode(ErrorType errorType) => errorType switch
    {
        ErrorType.Validation or ErrorType.Problem => StatusCodes.Status400BadRequest,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorType.Forbidden => StatusCodes.Status403Forbidden,
        _ => StatusCodes.Status500InternalServerError
    };
    
    private static Dictionary<string, object?>? GetErrors(Result result)
    {
        if (result.Error is not ValidationError validationError)
        {
            return null;
        }

        return new Dictionary<string, object?>
        {
            { "errors", validationError.Errors }
        };
    }
}
