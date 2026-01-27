using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Boilerplatr.Extensions;

public static class HttpContextExtensions
{
    public static bool HasAnonymousAttribute(this HttpContext context)
    {
        return context.GetEndpointCustomAttributes<AllowAnonymousAttribute>() is not null;
    }

    public static IReadOnlyList<T> GetEndpointCustomAttributes<T>(this HttpContext context) where T : class
    {
        return context.GetEndpoint()?.Metadata?.GetOrderedMetadata<T>() ?? [];
    }

    public static void UnauthorizedResponse(this HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
    }

    public static void ForbiddenResponse(this HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
    }

    public static bool TryGetCookie(this HttpContext context, string cookieName, [NotNullWhen(true)] out string? token)
    {
        return context.Request.Cookies.TryGetValue(cookieName, out token);
    }

    public static bool TryGetAccessHeader(this HttpContext context, [NotNullWhen(true)] out string? token)
    {
        token = context.Request.Headers.Authorization.ToString().Split(' ').ElementAtOrDefault(1);
        return !string.IsNullOrWhiteSpace(token);
    }
}
