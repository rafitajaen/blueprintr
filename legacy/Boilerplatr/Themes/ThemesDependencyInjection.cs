using Boilerplatr.Extensions;
using Microsoft.AspNetCore.Builder;

namespace Boilerplatr.Themes;

public static class ThemesDependencyInjection
{
    public static WebApplicationBuilder AddThemes(this WebApplicationBuilder builder)
    {
        builder.RegisterRequiredOptions<ThemesOptions, ThemesOptionsValidation>();
        return builder;
    }

    public static IApplicationBuilder UseThemes(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ThemesMiddleware>();
    }
}
