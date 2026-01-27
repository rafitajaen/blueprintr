using Boilerplatr.Abstractions.Entities;
using Boilerplatr.Extensions;
using Boilerplatr.Security.JwtBearerToken;
using System.Security.Claims;

namespace Boilerplatr.Security.Sessions;

public class Session : CacheableEntity<Session, string>
{
    public override string Id { get; init; }
    public string UserId { get; init; }
    public string UserEmail { get; init; }
    public string UserRole { get; init; }
    public string AccessTokenId { get; init; }
    public string RefreshTokenId { get; init; }

    public Session() {}

    private Session(string id, string? userId, string? userEmail, string? userRole, string accessTokenId, string refreshTokenId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        ArgumentException.ThrowIfNullOrWhiteSpace(userEmail);
        ArgumentException.ThrowIfNullOrWhiteSpace(userRole);
        ArgumentException.ThrowIfNullOrWhiteSpace(accessTokenId);
        ArgumentException.ThrowIfNullOrWhiteSpace(refreshTokenId);

        Id = id;
        UserId = userId;
        UserEmail = userEmail;
        UserRole = userRole;
        AccessTokenId = accessTokenId;
        RefreshTokenId = refreshTokenId;
    }

    public static Session CreateSession(string? userId, string? userEmail, string? userRole) => new
    (
        id: Guid.NewGuid().ToString(),
        userId: userId,
        userEmail: userEmail,
        userRole: userRole,
        accessTokenId: Guid.NewGuid().ToString(),
        refreshTokenId: Guid.NewGuid().ToString()
    );

    public static Session RenewSession(Session session) => new
    (
        id: session.Id,
        userId: session.UserId,
        userEmail: session.UserEmail,
        userRole: session.UserRole,
        accessTokenId: Guid.NewGuid().ToString(),
        refreshTokenId: Guid.NewGuid().ToString()
    );

    public static Session RenewSession(string sessionId, string? userId, string? userEmail, string? userRole) => new
    (
        id: sessionId,
        userId: userId,
        userEmail: userEmail,
        userRole: userRole,
        accessTokenId: Guid.NewGuid().ToString(),
        refreshTokenId: Guid.NewGuid().ToString()
    );

    public bool IsCandidateForRenovation(ClaimsIdentity accessTokenClaims, ClaimsIdentity refreshTokenClaims)
    {
        var isValid = accessTokenClaims.Claims.Count() == refreshTokenClaims.Claims.Count()

        /* Check SessionId */
            && string.Equals(Id, accessTokenClaims.FindValue(CustomClaims.SessionId), StringComparison.OrdinalIgnoreCase)
            && string.Equals(Id, refreshTokenClaims.FindValue(CustomClaims.SessionId), StringComparison.OrdinalIgnoreCase)

        /* Check UserId */
            && string.Equals(UserId, accessTokenClaims.FindValue(CustomClaims.UserId), StringComparison.OrdinalIgnoreCase)
            && string.Equals(UserId, refreshTokenClaims.FindValue(CustomClaims.UserId), StringComparison.OrdinalIgnoreCase)

        /* Check UserEmail */
            && string.Equals(UserEmail, accessTokenClaims.FindValue(CustomClaims.UserEmail), StringComparison.OrdinalIgnoreCase)
            && string.Equals(UserEmail, refreshTokenClaims.FindValue(CustomClaims.UserEmail), StringComparison.OrdinalIgnoreCase);

        return isValid;
    }
}
