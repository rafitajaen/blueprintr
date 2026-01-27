using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Boilerplatr.Extensions;
using Boilerplatr.Security.AccessToken;

namespace Boilerplatr.Security.Authorization;

public class AuthorizationMiddleware
(
    RequestDelegate next,
    IOptionsMonitor<AccessTokenOptions> options
)
{
    public async Task InvokeAsync(HttpContext context, CancellationToken cancellationToken)
    {
        if (context.HasAnonymousAttribute())
        {
            await next(context);
            return;
        }

        var isAuthorized = true;
        var authorizeAttributes = context.GetEndpointCustomAttributes<IAuthorizeAttribute>();

        /* Authorized Roles */
        var requiredRoles = authorizeAttributes.SelectMany(attribute => attribute.Roles);
        if (requiredRoles.Any())
        {
            var roles = context.User.GetArrayValues(options.CurrentValue.RoleClaimType) ?? [];

            isAuthorized &= requiredRoles.ContainsAnyIn(roles);
        }

        /* Authorized Permissions */
        var requiredPermissions = authorizeAttributes.SelectMany(attribute => attribute.Permissions);
        if (isAuthorized && requiredPermissions.Any())
        {
            var permissions = context.User.GetArrayValues(options.CurrentValue.PermissionsClaimType) ?? [];

            isAuthorized &= requiredPermissions.ContainsAllIn(permissions);
        }

        /* Authorized Claims */
        var requiredClaims = authorizeAttributes.SelectMany(attribute => attribute.Claims);
        if (isAuthorized && requiredClaims.Any())
        {
            isAuthorized &= requiredClaims.All(claim => context.User.FindFirst(claim) is not null);
        }

        if (isAuthorized)
        {
            var feature = new AuthorizationFeatures
            (
                IsAnonymous: false,
                IsAuthenticated: true,
                IsAuthorized: isAuthorized,
                RequiredRoles: requiredRoles,
                RequiredPermissions: requiredPermissions,
                RequiredClaims: requiredClaims
            );

            context.Features.Set(feature);
            await next(context);
        }
        else
        {
            context.ForbiddenResponse();
        }
    }
}
