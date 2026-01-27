using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Boilerplatr.Options;

public abstract class ICustomValidateOptions<TOptions>
(
    IConfiguration configuration
) : IValidateOptions<TOptions> where TOptions : ICustomOptions<TOptions>
{
    public static string SectionName => typeof(TOptions).Name.Replace("Options", "");
    public TOptions? Options { get; } = configuration.GetRequiredSection(SectionName).Get<TOptions>();
    public IConfiguration Configuration { get; } = configuration;

    public abstract ValidateOptionsResult Validate(string? name, TOptions options);

    protected ValidateOptionsResult FailIfEmpty(string propertyName) => ValidateOptionsResult.Fail($"{SectionName}:{propertyName} - This field cannot be empty.");
    protected ValidateOptionsResult FailIfNull(string propertyName) => ValidateOptionsResult.Fail($"{SectionName}:{propertyName} - This field cannot be null.");
    protected ValidateOptionsResult FailIfNotAbsoluteUri(string propertyName) => ValidateOptionsResult.Fail($"{SectionName}:{propertyName} - It's mandatory to provide a valid absolute Uri for this field.");
    protected ValidateOptionsResult FailIfInvalidEnum(string propertyName, Type enumType) => ValidateOptionsResult.Fail($"{SectionName}:{propertyName} - This field must have any valid value in EnumType <{enumType.Name}>. Valid values: {Enum.GetNames(enumType)}");
    protected ValidateOptionsResult FailIfNegative(string propertyName) => ValidateOptionsResult.Fail($"{SectionName}:{propertyName} - This field must be positive");
    protected ValidateOptionsResult FailIfGreaterThan(string propertyName, int value) => ValidateOptionsResult.Fail($"{SectionName}:{propertyName} - This field must be less than: {value}");
}
