using Boilerplatr.Security.JwtBearerToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace Boilerplatr.Endpoints;

public static class EndpointExtensions
{
    public static WebApplication MapEndpoints(this WebApplication app, Assembly assembly)
    {
        var endpointInterface = typeof(EndpointDefinition);

        var endpoints = assembly
                            .GetTypes()
                            .Where(t => endpointInterface.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        // Crear instancias de esas clases
        foreach (var type in endpoints)
        {
            var instance = Activator.CreateInstance(type);
            if (instance is EndpointDefinition endpoint)
            {
                app.MapEndpoint(endpoint);
            }
        }

        return app;
    }

    private static WebApplication MapEndpoint(this WebApplication app, EndpointDefinition endpoint)
    {
        var route = app.MapMethods(endpoint.Route, [endpoint.Verb], endpoint.RequestHandler);

        if (endpoint.RequiresAuthorization())
        {
            var options = app.Services.GetRequiredService<IOptions<AuthorizationOptions>>();
            var jwtOptions = app.Services.GetRequiredService<IOptions<JwtBearerTokenOptions>>();

            endpoint.AddSecurityPolicy(options.Value, jwtOptions.Value);

            route.RequireAuthorization
            (
                authorizeData: new AuthorizeAttribute()
                {
                    Roles = string.Join(',', endpoint.AllowedRoles),
                    Policy = endpoint.GetSecurityPolicyName(),
                }
            );
        }
        else if (!endpoint.RequiresAuthentication())
        {
            route.AllowAnonymous();
        }

        return app;
    }

    public static TBuilder RequireCustomAuthorization<TBuilder>(this TBuilder builder) where TBuilder : IEndpointConventionBuilder
    {
        return builder;
    }
    // public static TBuilder RequireCustomAuthorization<TBuilder>(this TBuilder builder, params ICustomAuthorizeData[] authorizeData) where TBuilder : IEndpointConventionBuilder
    // {
    //     return builder.WithMetadata(authorizeData);
    // }

}
