using System.Collections.Frozen;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Boilerplatr.Security.SecurityHeaders;

file static class SecurityHeadersExtensions
{
    public static readonly FrozenDictionary<string, string> SecurityHeaders = new Dictionary<string, string>()
    {
        { "X-Frame-Options", "DENY" },
        { "X-Content-Type-Options", "nosniff" },
        { "X-XSS-Protection", "1; mode=block" },
        { "Referrer-Policy", "strict-origin-when-cross-origin" },
        { "Strict-Transport-Security", "max-age=63072000; includeSubDomains; preload" },
    }.ToFrozenDictionary();

    public static readonly FrozenDictionary<string, string> DefaultContentSecurityPolicies = new Dictionary<string, string>()
    {
        { "default-src", "'none'" },
        { "script-src", "'self'" },
        { "style-src", "'self'" },
        { "img-src", "'self' data:" },
        { "font-src", "'self'" },
        { "connect-src", "'self'" },
        { "media-src", "'self'" },
        { "frame-ancestors", "'self'" },
        { "form-action", "'self'" },
    }.ToFrozenDictionary();

    public static string ToHeaderString(this IDictionary<string, string> dictionary)
    {
        var sb = new StringBuilder();

        foreach (var kvp in dictionary)
        {
            sb.Append(kvp.Key).Append(' ').Append(kvp.Value).Append("; ");
        }

        return sb.ToString();
    }

    public static string DefaultContentSecurityPolicy = DefaultContentSecurityPolicies.ToHeaderString();
}

public class SecurityHeadersMiddleware
(
    RequestDelegate next
)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Keep the original response body stream to restore it later
        var originalBody = context.Response.Body;

        // Use a memory stream to buffer the response
        using var buffer = new MemoryStream();
        context.Response.Body = buffer;

        // Continue down the middleware pipeline
        await next(context);

        // Rewind the buffer to the beginning so we can read it
        buffer.Seek(0, SeekOrigin.Begin);

        foreach (var header in SecurityHeadersExtensions.SecurityHeaders)
        {
            context.Response.Headers[header.Key] = header.Value;
        }

        var source = context.Items["Analytics:Source"]?.ToString();
        var csp = SecurityHeadersExtensions.DefaultContentSecurityPolicies.ToDictionary();

        if (!string.IsNullOrWhiteSpace(source))
        {
            csp["script-src"] += $" {source} ";
            csp["connect-src"] += $" {source} ";
        }

        var hashes = context.Items
            .Where(kvp => kvp.Key is string key && key.StartsWith("sha256-"))
            .Select(kvp => $"'{kvp.Key}'");

        if (hashes.Any())
        {
            csp["script-src"] += $" 'unsafe-hashes' {string.Join(' ', hashes)} ";
        }

        context.Response.Headers.ContentSecurityPolicy = csp.ToHeaderString();

        // Rewind and copy the buffered content back to the original stream
        buffer.Seek(0, SeekOrigin.Begin);
        await buffer.CopyToAsync(originalBody);

        // Restore the original response body stream
        context.Response.Body = originalBody;
    }
}
