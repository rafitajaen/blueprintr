using Boilerplatr.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Boilerplatr.Themes;

internal sealed class ThemesMiddleware
(
    RequestDelegate next,
    IOptions<ThemesOptions> options
)
{
    public async Task InvokeAsync(HttpContext context, CancellationToken cancellationToken)
    {
        context.Items.Add("ThemeEnabled", options.Value.Enabled);
        
        if (context.TryGetCookie(options.Value.CookieName, out var value) is not true || !ThemeCodes.IsValid(value) || !options.Value.Enabled)
        {
            context.Response.Cookies.Delete(options.Value.CookieName);
            var cookiesOptions = new CookieOptions()
            {
                HttpOnly = false,
                Expires = DateTimeOffset.Now.AddYears(1),
                SameSite = SameSiteMode.Strict,
                Secure = true
            };

            value = options.Value.Enabled ? options.Value.DefaultTheme : ThemeCodes.Light;
            context.Response.Cookies.Append(options.Value.CookieName, value, cookiesOptions);
        }

        context.Items.Add("Theme", value);

        await next(context);
    }
}
