using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Boilerplatr.Shared;

public static class RazorDependencyInjection
{
    public static WebApplicationBuilder AddRazorComponents(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorComponents();

        return builder;
    }

    public static WebApplication UseRazorComponents<TComponent>(this WebApplication app, Assembly? additionalAssembly = null)
    {
        app.UseAntiforgery();
        app.MapStaticAssets();

        // app.UseRazorComponents<App>();
        
        var builder = app.MapRazorComponents<TComponent>();

        if (additionalAssembly is not null)
        {
            builder.AddAdditionalAssemblies(additionalAssembly);
        }

        return app;
    }
}
