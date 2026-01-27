using Boilerplatr.Abstractions.Errors;
using Boilerplatr.Abstractions.Results;

namespace Boilerplatr.Abstractions;

public sealed record ValidationError : Error
{
    public ValidationError(Error[] errors) : base
    (
        code: "Validation.General",
        field: "Validation.General",
        description: "One or more validation errors occurred",
        type: ErrorType.Validation
    )
    {
        Errors = errors;
    }

    public Error[] Errors { get; }

    public static ValidationError FromResults(IEnumerable<Result> results) => new
    (
        errors: results.Where(r => r.IsFailure).Select(r => r.Error).ToArray()
    );
}
