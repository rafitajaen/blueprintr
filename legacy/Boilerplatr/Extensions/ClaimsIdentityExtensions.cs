using System.Security.Claims;

namespace Boilerplatr.Extensions;

public static class ClaimsIdentityExtensions
{
    public static string? FindValue(this ClaimsIdentity claims, string type) => claims.FindFirst(type)?.Value;
    public static IEnumerable<string>? GetArrayValues(this ClaimsPrincipal claims, string type) => claims.FindFirst(type)?.Value.Split(',').Select(x => x.Trim());
}
