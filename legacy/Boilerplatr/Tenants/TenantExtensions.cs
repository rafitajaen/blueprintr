using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Boilerplatr.Tenants;

public static class TenantExtensions
{
    public static Tenant GetTenant(this HttpContext context) => context.Features.GetRequiredFeature<TenantFeatures>().Tenant;
}
