using Boilerplatr.Security.Authentication;
using Boilerplatr.Security.Authorization;
using Boilerplatr.Security.SecurityHeaders;
using Boilerplatr.Security.Sessions;
using Microsoft.AspNetCore.Builder;

namespace Boilerplatr.Security;

public static class SecurityDependencyInjection
{
    public static WebApplicationBuilder AddCustomSecurity(this WebApplicationBuilder builder)
    {
        return builder
                .AddSessions()
                .AddCustomAuthentication()
                .AddCustomAuthorization();
    }

    public static IApplicationBuilder UseCustomSecurity<TIdentity, T>(this IApplicationBuilder app)
    {
        return app
                .UseCustomAuthentication<TIdentity, T>()
                .UseCustomAuthorization()
                .UseSecurityHeaders();
    }
}
