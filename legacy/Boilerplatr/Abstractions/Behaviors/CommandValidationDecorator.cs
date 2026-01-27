using Boilerplatr.Abstractions.Errors;
using Boilerplatr.Abstractions.Messaging;
using Boilerplatr.Abstractions.Results;
using FluentValidation;
using FluentValidation.Results;

namespace Boilerplatr.Abstractions.Behaviors;

internal static class CommandValidationDecorator
{
    internal sealed class CommandHandler<TCommand, TResponse>
    (
        ICommandHandler<TCommand, TResponse> innerHandler,
        IEnumerable<IValidator<TCommand>> validators
    ) : ICommandHandler<TCommand, TResponse> where TCommand : ICommand<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
        {
            ValidationFailure[] validationFailures = await ValidateAsync(command, validators);

            if (validationFailures.Length == 0)
            {
                return await innerHandler.Handle(command, cancellationToken);
            }

            return Result.Failure<TResponse>(CreateValidationError(validationFailures));
        }
    }

    internal sealed class CommandBaseHandler<TCommand>
    (
        ICommandHandler<TCommand> innerHandler,
        IEnumerable<IValidator<TCommand>> validators
    ) : ICommandHandler<TCommand> where TCommand : ICommand
    {
        public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
        {
            ValidationFailure[] validationFailures = await ValidateAsync(command, validators);

            if (validationFailures.Length == 0)
            {
                return await innerHandler.Handle(command, cancellationToken);
            }

            return Result.Failure(CreateValidationError(validationFailures));
        }
    }

    private static async Task<ValidationFailure[]> ValidateAsync<TCommand>
    (
        TCommand command,
        IEnumerable<IValidator<TCommand>> validators
    )
    {
        if (!validators.Any())
        {
            return [];
        }

        var context = new ValidationContext<TCommand>(command);

        ValidationResult[] validationResults = await Task.WhenAll
        (
            tasks: validators.Select(validator => validator.ValidateAsync(context))
        );

        ValidationFailure[] validationFailures = validationResults
            .Where(validationResult => !validationResult.IsValid)
            .SelectMany(validationResult => validationResult.Errors)
            .ToArray();

        return validationFailures;
    }

    private static ValidationError CreateValidationError(ValidationFailure[] validationFailures) =>
        new(validationFailures.Select(f => Error.Problem(f.ErrorCode, f.PropertyName, f.ErrorMessage)).ToArray());
}
