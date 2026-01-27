using Boilerplatr.Identities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Boilerplatr.Security.Authentication;

internal sealed class AuthenticationMiddleware<TIdentity, T>
(
    RequestDelegate next,
    IAuthenticationService authenticationService
)
{
    public async Task InvokeAsync(HttpContext context, CancellationToken cancellationToken)
    {
        if (context.HasAnonymousAttribute())
        {
            await next(context);
            return;
        }

        if (!await authenticationService.TryAuthenticate(context, cancellationToken))
        {
            await HandleUnauthorizedAsync(context, cancellationToken);
            return;
        }

        context.Items["IsAuthenticated"] = true;

        var repository = context.RequestServices.GetRequiredService<IIdentityRepository<TIdentity, T>>();

        if (repository.TryGetUserId(context, out var id) && await repository.GetAsync(id, cancellationToken) is TIdentity user)
        {
            context.Features.Set(new IdentityFeature<TIdentity>(user));
            await next(context);
        }
        else
        {
            await HandleUnauthorizedAsync(context, cancellationToken);
        }
    }

    private static async Task HandleUnauthorizedAsync(HttpContext context, CancellationToken cancellationToken)
    {
        var authService = context.RequestServices.GetRequiredService<IAuthenticationService>();
        await authService.DeleteCookiesAsync(context, cancellationToken);

        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.UnauthorizedResponse();
        }
        else
        {
            context.Response.Redirect($"/auth/login?returnUrl={context.Request.Path}");
        }
    }
}
