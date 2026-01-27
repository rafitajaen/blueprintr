using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

namespace Boilerplatr.Tenants;

public static class TenantDependencyInjection
{
    public static IApplicationBuilder UseTenants<TContext>(this IApplicationBuilder app) where TContext : DbContext
    {
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor
                | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
                | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedHost
        });
        
        return app.UseMiddleware<TenantMiddleware<TContext>>();
    }
}
