using Boilerplatr.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Boilerplatr.Security.Sessions;

public class SessionsOptions : ICustomOptions<SessionsOptions>
{
    public int SlidingExpirationMinutes { get; set; } = 1440;
}

public class SessionsOptionsValidation
(
    IConfiguration configuration
) : ICustomValidateOptions<SessionsOptions>(configuration)
{
    public override ValidateOptionsResult Validate(string? name, SessionsOptions options)
    {
        if (options.SlidingExpirationMinutes <= 0)
        {
            return FailIfNegative(propertyName: nameof(options.SlidingExpirationMinutes));
        }

        return ValidateOptionsResult.Success;
    }
}
