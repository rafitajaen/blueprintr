
using Boilerplatr.Extensions;
using Boilerplatr.Identities;
using Boilerplatr.Security.JwtBearerToken;
using Boilerplatr.Security.Sessions;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace Boilerplatr.Security.AccessToken;

public sealed class AccessTokenClaims
(
    string TokenId,
    string SessionId,
    string UserId,
    string UserEmail,
    string? UserRole
) : JwtBearerTokenClaims
{
    public string TokenId { get; } = TokenId;
    public string SessionId { get; } = SessionId;
    public string UserId { get; } = UserId;
    public string UserEmail { get; } = UserEmail;
    public string? UserRole { get; } = UserRole;

    public static AccessTokenClaims From(Session session)
    {
        return new
        (
            TokenId: session.AccessTokenId,
            SessionId: session.Id,
            UserId: session.UserId,
            UserEmail: session.UserEmail,
            UserRole: session.UserRole
        );
    }
}

public interface IAccessTokenProvider : IJwtBearerTokenProvider<AccessTokenOptions, AccessTokenClaims>;

internal sealed class AccessTokenProvider(IOptions<AccessTokenOptions> options) 
: JwtBearerTokenProvider<AccessTokenOptions, AccessTokenClaims>(options.Value), IAccessTokenProvider
{

    public override bool TryGenerateToken(AccessTokenClaims claims, out string token)
    {
        return TryGenerateToken
        (
            claims:
            [
                new (CustomClaims.TokenId, claims.TokenId),
                new (CustomClaims.SessionId, claims.SessionId),
                new (CustomClaims.UserId, claims.UserId),
                new (CustomClaims.UserEmail, claims.UserEmail),
                new (CustomClaims.UserRole, string.IsNullOrWhiteSpace(claims.UserRole) ? nameof(BaseRoles.Guest) : claims.UserRole),
            ],
            out token
        );
    }

    public override bool HasValidClaims(IEnumerable<Claim>? claims)
    {
        if (claims is null)
        {
            return false;
        }

        var email = claims.FindFirstValue(CustomClaims.UserEmail);

        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        var sessionId = claims.FindFirstValue(CustomClaims.SessionId);

        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return false;
        }

        var userId = claims.FindFirstValue(CustomClaims.UserId);

        if (string.IsNullOrWhiteSpace(userId))
        {
            return false;
        }

        var tokenId = claims.FindFirstValue(CustomClaims.TokenId);

        if (string.IsNullOrWhiteSpace(tokenId))
        {
            return false;
        }

        var role = claims.FindFirstValue(CustomClaims.UserRole);

        if (string.IsNullOrWhiteSpace(role))
        {
            return false;
        }

        return true;
    }

    public override bool TryGetClaims(string? token, [NotNullWhen(true)] out AccessTokenClaims? claims)
    {
        claims = null;

        try
        {
            var identity = ExtractClaimsIdentity(token);
            claims = new
            (
                TokenId: identity.FindValue(CustomClaims.TokenId) ?? throw new Exception(),
                SessionId: identity.FindValue(CustomClaims.SessionId) ?? throw new Exception(),
                UserId: identity.FindValue(CustomClaims.UserId) ?? throw new Exception(),
                UserEmail: identity.FindValue(CustomClaims.UserEmail) ?? throw new Exception(),
                UserRole: identity.FindValue(CustomClaims.UserRole) ?? nameof(BaseRoles.Guest)
            );

            return true;
        }
        catch
        {
            return false;
        }
        
    }
}
