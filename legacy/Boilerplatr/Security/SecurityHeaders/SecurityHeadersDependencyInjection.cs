using Microsoft.AspNetCore.Builder;

namespace Boilerplatr.Security.SecurityHeaders;

public static class SecurityHeadersDependencyInjection
{
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SecurityHeadersMiddleware>();
    }
}
