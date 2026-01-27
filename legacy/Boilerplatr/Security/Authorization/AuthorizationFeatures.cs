namespace Boilerplatr.Security.Authorization;

public interface IAuthorizationFeatures
{
    bool IsAnonymous { get; init; }
    bool IsAuthenticated { get; init; }
    bool IsAuthorized { get; init; }
    IEnumerable<string> RequiredRoles { get; init; }
    IEnumerable<string> RequiredPermissions { get; init; }
    IEnumerable<string> RequiredClaims { get; init; }
}

public sealed record AuthorizationFeatures
(
    bool IsAnonymous,
    bool IsAuthenticated,
    bool IsAuthorized,
    IEnumerable<string> RequiredRoles,
    IEnumerable<string> RequiredPermissions,
    IEnumerable<string> RequiredClaims
) : IAuthorizationFeatures;

