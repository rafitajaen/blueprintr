
using Boilerplatr.Extensions;
using Boilerplatr.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Boilerplatr.Tenants;

internal sealed class TenantMiddleware<TContext>
(
    RequestDelegate next,
    ILogger<TenantMiddleware<TContext>> logger
) where TContext : DbContext
{
    public async Task InvokeAsync(HttpContext context, IWebHostEnvironment env, CancellationToken cancellationToken)
    {
        var host = context.Request.Host.Host.RemoveWwwPrefix();

        logger.LogForwardedHost(host);

        Tenant? tenant = null;

        using (var scope = context.RequestServices.CreateScope())
        {
            // TODO CACHE TENANTS
            var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();

            tenant = await dbContext.Set<Tenant>()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Host == host, cancellationToken);

            tenant ??= await dbContext.Set<Tenant>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.IsDefault, cancellationToken);

            tenant ??= await dbContext.Set<Tenant>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cancellationToken);
        }

        if (tenant is null)
        {
            context.ForbiddenResponse();
            return;
        }

        context.Features.Set(new TenantFeatures(tenant));

        context.Items.Add("Request:Path", context.Request.Path);
        context.Items.Add("WebHost:Environment", env.EnvironmentName);

        context.Items.Add("Tenant:Name", tenant.Name);
        context.Items.Add("Tenant:Class", tenant.Class);
        context.Items.Add("Tenant:Excerpt", tenant.Excerpt);
        context.Items.Add("Tenant:Description", tenant.Description);

        context.Items.Add("Tenant:Logo", tenant.Logo);
        context.Items.Add("Tenant:Host", context.Request.Host.Host);
        context.Items.Add("Tenant:ExposedUrl", tenant.GetExposedUrl());

        context.Items.Add("Tenant:SupportEmail", tenant.SupportEmail);
        context.Items.Add("Tenant:ContactEmail", tenant.ContactEmail);
        context.Items.Add("Tenant:NewsletterEmail", tenant.NewsletterEmail);
        context.Items.Add("Tenant:GoogleSiteVerification", tenant.GoogleSiteVerification);

        if (tenant.Analytics?.Enabled is true)
        {
            context.Items.Add("Analytics:Source", tenant.Analytics.Source);
            context.Items.Add("Analytics:Script", tenant.Analytics.Script);
        }

        await next(context);
    }
}
