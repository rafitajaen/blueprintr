using Boilerplatr.Abstractions.Entities;
using Boilerplatr.Emails.Options;
using Boilerplatr.Options;
using Boilerplatr.Shared;

namespace Boilerplatr.Tenants;

public sealed class Tenant : Entity<Guid7>
{
    public required override Guid7 Id { get; init; }

    public bool IsDefault { get; set; }
    public bool IsHttpsEnforced { get; set; }

    public required string Class { get; set; }
    public required string Host { get; set; }
    public int? Port { get; set; }
    public required string Name { get; set; }
    public required string Excerpt { get; set; }
    public required string Description { get; set; }
    public required string Logo { get; set; }
    public required string FaviconPath { get; set; }
    public string? GoogleSiteVerification { get; set; }

    // Emails
    public required string SupportEmail { get; set; }
    public required string NewsletterEmail { get; set; }
    public required string ContactEmail { get; set; }
    public required string NoReplyEmail { get; set; }

    // Owned Types
    public Social Social { get; set; } = new();
    public EmailOptions Email { get; set; } = new();
    public AnalyticsOptions Analytics { get; set; } = new();

    private string GetPort() => Port > 0 && Port <= 65535 ? $":{Port}" : string.Empty;

    public string GetExposedUrl()
    {
        var scheme = IsHttpsEnforced ? "https" : "http";
        return $"{scheme}://{Host}{GetPort()}";
    }
}
