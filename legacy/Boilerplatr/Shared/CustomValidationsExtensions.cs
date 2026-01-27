using FluentValidation;

namespace Boilerplatr.Shared;

public static class CustomValidationExtensions
{
    /// <summary>
    /// Validates that the string contains only letters, numbers, and underscores.
    /// </summary>
    public static IRuleBuilderOptions<T, string> Username<T>
    (
        this IRuleBuilder<T, string> ruleBuilder
    )
    {
        return ruleBuilder
            .NotNull()
            .NotEmpty()
            .Matches("^[a-zA-Z0-9_]+$")
            .WithMessage("Only letters, numbers, and underscores are allowed.")
            .MinimumLength(6)
            .MaximumLength(50);
    }

    public static IRuleBuilderOptions<T, string> Guid7<T>
    (
        this IRuleBuilder<T, string> ruleBuilder
    )
    {
        return ruleBuilder
            .NotNull()
            .NotEmpty()
            .Must(value => Shared.Guid7.TryParse(value, out _))
            .WithMessage("The value must be a valid GUID v7.");
    }
}
