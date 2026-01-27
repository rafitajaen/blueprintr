using Boilerplatr.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Boilerplatr.Extensions;

public static class ConfigurationExtensions
{
    // https://stackoverflow.com/questions/76736915/how-to-validate-options-during-startup-and-on-change-using-custom-validators-f
    public static WebApplicationBuilder RegisterRequiredOptions<TOptions, TValidation>
    (
        this WebApplicationBuilder builder,
        string? sectionName = null
    )
    where TOptions : ICustomOptions<TOptions>
    where TValidation : ICustomValidateOptions<TOptions>
    {
        var section = builder.Configuration.GetRequiredSection(sectionName ?? typeof(TOptions).GetOptionName());

        ArgumentNullException.ThrowIfNull(section);

        builder.Services
            .AddOptions<TOptions>()
            .Bind(section)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddTransient<IValidateOptions<TOptions>, TValidation>();

        return builder;
    }

    public static TOptions GetRegisteredOptions<TOptions>
    (
        this WebApplicationBuilder builder,
        string? sectionName = null
    )
    where TOptions : ICustomOptions<TOptions>
    {
        return builder.Configuration
                .GetRequiredSection(sectionName ?? typeof(TOptions).GetOptionName())
                .Get<TOptions>() ?? throw new NullReferenceException();
    }

    private static string GetOptionName(this Type t) => t.Name.Replace("Options", string.Empty);
}
