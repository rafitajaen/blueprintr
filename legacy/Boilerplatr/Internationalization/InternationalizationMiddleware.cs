using Boilerplatr.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;

namespace Boilerplatr.Internationalization;

public sealed class InternationalizationMiddleware
(
    RequestDelegate next,
    IOptions<InternationalizationOptions> options
)
{
    public static readonly CookieOptions LanguageCookieOptions = new()
    {
        HttpOnly = false,
        Expires = null,
        SameSite = SameSiteMode.Strict,
        Secure = true
    };

    public async Task InvokeAsync(HttpContext context, CancellationToken cancellationToken)
    {
        if (!context.Request.Path.StartsWithSegments("/api"))
        {
            string? value = null;

            if (options.Value.Enabled && options.Value.PlatformLanguages.Count > 1)
            {
                context.Items.Add("LanguageEnabled", options.Value.Enabled);
            }

            if (!context.TryGetCookie(options.Value.CookieName, out var language))
            {
                foreach (var kvp in context.Request.ExtractLanguageWeights())
                {
                    if (options.Value.PlatformLanguages.Contains(kvp.Key))
                    {
                        language = kvp.Key;
                        break;
                    }
                }

                language ??= options.Value.DefaultLanguage;
                value = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(language));
                context.Items.Add("Language", language);
            }
            else if (context.Features.Get<IRequestCultureFeature>() is IRequestCultureFeature feature)
            {
                value = CookieRequestCultureProvider.MakeCookieValue(feature.RequestCulture);
                context.Items.Add("Language", feature.RequestCulture.Culture);
            }

            if (!string.IsNullOrWhiteSpace(value))
            {
                context.Response.Cookies.Delete(options.Value.CookieName);
                context.Response.Cookies.Append
                (
                    key: options.Value.CookieName,
                    value: value,
                    options: LanguageCookieOptions
                );
            }
        }

        await next(context);
    }
}
