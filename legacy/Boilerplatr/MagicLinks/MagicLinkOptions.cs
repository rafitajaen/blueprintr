using Boilerplatr.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Boilerplatr.MagicLinks;

public class MagicLinkOptions : ICustomOptions<MagicLinkOptions>
{
    public string BaseUrl { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; } = 60;
}

public class MagicLinkOptionsValidation
(
    IConfiguration configuration
) : ICustomValidateOptions<MagicLinkOptions>(configuration)
{
    public override ValidateOptionsResult Validate(string? name, MagicLinkOptions options)
    {
        // TODO: Complete Validation

        return ValidateOptionsResult.Success;
    }
}

