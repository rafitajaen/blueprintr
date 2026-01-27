using Boilerplatr.Security.AccessToken;
using Boilerplatr.Security.RefreshToken;
using Boilerplatr.Security.Sessions;
using Microsoft.AspNetCore.Http;
using Boilerplatr.Extensions;
using NodaTime;
using Microsoft.IdentityModel.Tokens;
using Boilerplatr.Security.JwtBearerToken;
using System.Security.Claims;

namespace Boilerplatr.Security.Authentication;

public interface IAuthenticationService
{
    Task DeleteCookiesAsync(HttpContext context, CancellationToken cancellationToken = default);
    Task<bool> TryLogin(HttpContext context, string? userId, string? userEmail, string? userRole, CancellationToken cancellationToken = default);
    Task<bool> TryAuthenticate(HttpContext context, CancellationToken cancellationToken = default);
}

internal sealed class AuthenticationService
(
    ISessionsService<Session, string> sessions,
    IAccessTokenProvider access,
    IRefreshTokenProvider refresh
) : IAuthenticationService
{
    public async Task<bool> TryLogin(HttpContext context, string? userId, string? userEmail, string? userRole, CancellationToken cancellationToken = default)
    {
        var session = Session.CreateSession
        (
            userId: userId,
            userEmail: userEmail,
            userRole: userRole
        );

        return await TrySetCookiesAsync(context, session, cancellationToken);
    }

    public async Task<bool> TryAuthenticate(HttpContext context, CancellationToken cancellationToken = default)
    {
        var isAuthenticated = false;

        if (access.TryGetFromHeader(context, out string? accessToken) || access.TryGetFromCookie(context, out accessToken))
        {
            var tokenValidation = await access.ValidateAsync(accessToken);

            if (tokenValidation.IsValid())
            {
                isAuthenticated = true;
            }
            else if (tokenValidation.IsExpired() && refresh.TryGetFromCookie(context, out var refreshToken))
            {
                isAuthenticated = await TryRenewAsync(context, tokenValidation, refreshToken, cancellationToken);
            }

            if (isAuthenticated)
            {
                context.User = new ClaimsPrincipal(tokenValidation.ClaimsIdentity);
            }
            else
            {
                await DeleteCookiesAsync(context, cancellationToken);
            }
        }
        else if (refresh.TryGetFromCookie(context, out var refreshToken))
        {
            var tokenValidation = await refresh.ValidateAsync(refreshToken);

            isAuthenticated = await TryRenewAsync(context, tokenValidation, refreshToken, cancellationToken);

            if (isAuthenticated)
            {
                context.User = new ClaimsPrincipal(tokenValidation.ClaimsIdentity);
            }
            else
            {
                await DeleteCookiesAsync(context, cancellationToken);
            }

        }

        return isAuthenticated;
    }

    private async Task<bool> TrySetCookiesAsync(HttpContext context, Session session, CancellationToken cancellationToken = default)
    {
        if (access.TryGenerateToken(AccessTokenClaims.From(session), out var accessToken) && refresh.TryGenerateToken(RefreshTokenClaims.From(session), out var refreshToken))
        {
            await DeleteCookiesAsync(context, cancellationToken);

            await sessions.SetAsync(session, cancellationToken: cancellationToken);

            var now = SystemClock.Instance.GetCurrentInstant();

            context.Response.Cookies.Append(access.Options.CookieName, accessToken, access.GetCookieOptions(now));
            context.Response.Cookies.Append(refresh.Options.CookieName, refreshToken, refresh.GetCookieOptions(now));

            context.User = new ClaimsPrincipal(access.ExtractClaimsIdentity(accessToken));

            return true;
        }

        return false;
    }

    private async Task<bool> TryRenewAsync(HttpContext context, TokenValidationResult accessTokenValidation, string? refreshToken, CancellationToken cancellationToken = default)
    {
        var refreshTokenValidation = await refresh.ValidateAsync(refreshToken);
        if (refreshTokenValidation.IsValid() is false)
        {
            return false;
        }

        var sessionId = refreshTokenValidation.ClaimsIdentity.FindValue(CustomClaims.SessionId);
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return false;
        }

        var session = await sessions.GetAsync(sessionId, cancellationToken);

        if (session is not null && session.IsCandidateForRenovation(accessTokenValidation.ClaimsIdentity, refreshTokenValidation.ClaimsIdentity))
        {
            session = Session.RenewSession(session);
        }

        session ??= Session.CreateSession
        (
            userId: refreshTokenValidation.ClaimsIdentity.FindValue(CustomClaims.UserId),
            userEmail: refreshTokenValidation.ClaimsIdentity.FindValue(CustomClaims.UserEmail),
            userRole: refreshTokenValidation.ClaimsIdentity.FindValue(CustomClaims.UserRole)
        );

        return await TrySetCookiesAsync(context, session, cancellationToken);
    }

    public Task DeleteCookiesAsync(HttpContext context, CancellationToken cancellationToken = default)
    {
        context.Response.Cookies.Delete(access.Options.CookieName);
        context.Response.Cookies.Delete(refresh.Options.CookieName);

        return Task.CompletedTask;
    }
}
