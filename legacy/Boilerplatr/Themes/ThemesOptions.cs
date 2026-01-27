using Boilerplatr.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Boilerplatr.Themes;

public class ThemesOptions : ICustomOptions<ThemesOptions>
{
    public bool Enabled { get; set; }
    public string CookieName { get; set; } = "boilerplatr-theme";
    public string DefaultTheme { get; set; } = ThemeCodes.Light;
    public HashSet<string> PlatformThemes { get; set; } = [..ThemeCodes.All];
}

public class ThemesOptionsValidation
(
    IConfiguration configuration
) : ICustomValidateOptions<ThemesOptions>(configuration)
{
    public override ValidateOptionsResult Validate(string? name, ThemesOptions options)
    {
        if (options.Enabled)
        {
            if (string.IsNullOrWhiteSpace(options.CookieName))
            {
                return FailIfEmpty(propertyName: nameof(options.CookieName));
            }

            if (string.IsNullOrWhiteSpace(options.DefaultTheme))
            {
                return FailIfEmpty(propertyName: nameof(options.DefaultTheme));
            }
            else if (!ThemeCodes.IsValid(options.DefaultTheme))
            {
                return ValidateOptionsResult.Fail($"{SectionName}:{nameof(options.DefaultTheme)} - It has not a value in available themes: [{string.Join(", ", ThemeCodes.All)}]");
            }

            if (!options.PlatformThemes.Any())
            {
                return FailIfEmpty(propertyName: nameof(options.PlatformThemes));
            }
            else if (!options.PlatformThemes.Contains(options.DefaultTheme))
            {
                ValidateOptionsResult.Fail($"{SectionName}:{nameof(options.PlatformThemes)} - It does not contains default theme: [{options.DefaultTheme}]");
            }
            else if (options.PlatformThemes.Count == 1)
            {
                ValidateOptionsResult.Fail($"{SectionName}:{nameof(options.PlatformThemes)} - Must have more than one value. Available Themes: [{string.Join(", ", ThemeCodes.All)}]");
            }
        }

        return ValidateOptionsResult.Success;
    }
}
