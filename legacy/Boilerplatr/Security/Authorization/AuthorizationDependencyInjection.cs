using Microsoft.AspNetCore.Builder;

namespace Boilerplatr.Security.Authorization;

public static class AuthorizationDependencyInjection
{
    public static WebApplicationBuilder AddCustomAuthorization(this WebApplicationBuilder builder)
    {
        return builder;
    }

    public static IApplicationBuilder UseCustomAuthorization(this IApplicationBuilder app)
    {
        return app.UseMiddleware<AuthorizationMiddleware>();
    }
}