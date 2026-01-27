using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Boilerplatr.Mvc;

public static class MvcDependencyInjection
{
    public static WebApplicationBuilder AddCustomMvc<TLocationExpander>(this WebApplicationBuilder builder, TLocationExpander expander)  where TLocationExpander : IViewLocationExpander
    {
        // builder.WebHost.UseStaticWebAssets();
        // builder.Services.AddAntiforgery();

        var web = typeof(TLocationExpander).Assembly;
        var part = new AssemblyPart(web);

        var mvc = builder.Services
            // .AddControllersWithViews()
            .AddMvcCore()
            .AddViews()
            .AddRazorViewEngine(o => o.ViewLocationExpanders.Add(expander))
            .ConfigureApplicationPartManager(apm => apm.ApplicationParts.Add(part));
        // .AddRazorOptions(o => o.ViewLocationExpanders.Add(expander));
        // .AddControllersAsServices()
        // .AddRazorRuntimeCompilation(o => o.FileProviders.Add(new EmbeddedFileProvider(web)));
            
        mvc.AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        // builder.Services.Configure<RazorViewEngineOptions>(o => o.ViewLocationExpanders.Add(expander));

        // if (builder.Environment.IsDevelopment())
        // {
        //     mvc.AddRazorPages().AddRazorRuntimeCompilation(o => o.FileProviders.Add(new EmbeddedFileProvider(web)));

        //     builder.Services.Configure<MvcRazorRuntimeCompilationOptions>(options =>
        //     {
        //         var libraryPath = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "src", "DotnetToday.Web", "Layouts"));
        //         options.FileProviders.Add(new PhysicalFileProvider(libraryPath));
        //     });
        // }

        return builder;
    }

    public static WebApplication UseCustomMvc(this WebApplication app, Assembly? additionalAssembly = null)
    {
        // app.UseAntiforgery();
        // app.MapStaticAssets();
        app.MapControllers();
        // var builder = app.MapRazorComponents<TComponent>();

        // if (additionalAssembly is not null)
        // {
        //     builder.AddAdditionalAssemblies(additionalAssembly);
        // }

        return app;
    }
}
