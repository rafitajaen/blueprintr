using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Boilerplatr.Security.Authentication;

public interface IAuthenticationFeatures
{
    bool IsAnonymous { get; init; }
    bool IsAuthenticated { get; init; }
    string Token { get; init; }
    ClaimsIdentity Claims { get; init; }
}

public sealed record AuthenticationFeatures
(
    bool IsAnonymous,
    bool IsAuthenticated,
    string Token,
    ClaimsIdentity Claims
) : IAuthenticationFeatures;

public static class AuthenticationFeaturesExtensions
{
    public static AuthenticationFeatures? GetAuthenticationFeatures(this HttpContext context) => context.Features.Get<AuthenticationFeatures>();
    public static bool IsAuthenticated(this HttpContext context) => context.GetAuthenticationFeatures()?.IsAuthenticated is true;
}

