namespace Boilerplatr.Tenants;

public interface ITenantFeatures
{
    Tenant Tenant { get; init; }
}

public sealed record TenantFeatures
(
    Tenant Tenant
) : ITenantFeatures;

