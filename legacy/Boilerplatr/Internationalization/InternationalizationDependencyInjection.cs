using Boilerplatr.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Boilerplatr.Internationalization;

public static class InternationalizationDependencyInjection
{
    public static WebApplicationBuilder AddInternationalization(this WebApplicationBuilder builder, string? resourcesPath = default)
    {
        builder.RegisterRequiredOptions<InternationalizationOptions, InternationalizationOptionsValidation>();

        if (resourcesPath is null)
        {
            builder.Services.AddLocalization();
        }
        else
        {
            builder.Services.AddLocalization(options => options.ResourcesPath = resourcesPath);
        }

        // builder.Services
        //     .AddControllersWithViews()
        //     .AddViewLocalization()
        //     .AddDataAnnotationsLocalization();

        return builder;
    }

    public static IApplicationBuilder UseInternationalization(this IApplicationBuilder app)
    {
        var options = app.ApplicationServices.GetRequiredService<IOptions<InternationalizationOptions>>();
        var localizationOptions = new RequestLocalizationOptions()
            .SetDefaultCulture(options.Value.DefaultLanguage)
            .AddSupportedCultures([.. options.Value.PlatformLanguages])
            .AddSupportedUICultures([.. options.Value.PlatformLanguages]);
            // .AddInitialRequestCultureProvider(new AcceptLanguageHeaderRequestCultureProvider());

        localizationOptions.RequestCultureProviders =
        [
            new CookieRequestCultureProvider()
            {
                CookieName = options.Value.CookieName
            },
            new AcceptLanguageHeaderRequestCultureProvider()
        ];

        app.UseRequestLocalization(localizationOptions);

        return app.UseMiddleware<InternationalizationMiddleware>();
    }
}
