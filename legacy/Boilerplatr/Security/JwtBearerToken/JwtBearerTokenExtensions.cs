using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Boilerplatr.Security.JwtBearerToken;

public static class JwtBearerTokenExtensions
{
    public static bool IsValid(this TokenValidationResult result) => result.IsValid;
    public static bool IsExpired(this TokenValidationResult result) => result.Exception?.Message.Contains("IDX10223") is true;
    public static bool IsDesynchronized(this TokenValidationResult result) => throw new NotImplementedException();
    public static string? FindFirstValue(this TokenValidationResult tokenValidationResult, string claimType)
    {
        return tokenValidationResult
                .ClaimsIdentity
                .FindFirst(claim => string.Equals(claim.Type, claimType, StringComparison.OrdinalIgnoreCase))?
                .Value;
    }

    public static string? FindFirstValue(this IEnumerable<Claim> claims, string claimType)
    {
        return claims.FirstOrDefault(c => string.Equals(c.Type, claimType, StringComparison.OrdinalIgnoreCase))?.Value;
    }

}
