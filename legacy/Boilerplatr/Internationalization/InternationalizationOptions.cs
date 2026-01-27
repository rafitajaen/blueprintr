using Boilerplatr.Options;
using Boilerplatr.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Boilerplatr.Internationalization;

public class InternationalizationOptions : ICustomOptions<InternationalizationOptions>
{
    public bool Enabled { get; set; }
    public string CookieName { get; set; } = "boilerplatr-language";
    public string DefaultLanguage { get; set; } = LanguageCodes.English;
    public HashSet<string> PlatformLanguages { get; set; } = [];
}

public class InternationalizationOptionsValidation
(
    IConfiguration configuration
) : ICustomValidateOptions<InternationalizationOptions>(configuration)
{
    public override ValidateOptionsResult Validate(string? name, InternationalizationOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.CookieName))
        {
            return FailIfEmpty(propertyName: nameof(options.CookieName));
        }

        if (string.IsNullOrWhiteSpace(options.DefaultLanguage))
        {
            return FailIfEmpty(propertyName: nameof(options.DefaultLanguage));
        }

        if (options.PlatformLanguages.Count == 0)
        {
            return FailIfEmpty(propertyName: nameof(options.PlatformLanguages));
        }
        else if (options.PlatformLanguages.Any(string.IsNullOrWhiteSpace))
        {
            return ValidateOptionsResult.Fail("Internationalization Options :: PlatformLanguages contains empty language codes.");
        }
        else if (!options.PlatformLanguages.Contains(options.DefaultLanguage))
        {
            return ValidateOptionsResult.Fail("Internationalization Options :: DefaultLanguage is not in PlatformLanguages.");
        }

        return ValidateOptionsResult.Success;
    }
}
