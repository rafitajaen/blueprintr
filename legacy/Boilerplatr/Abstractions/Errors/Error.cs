namespace Boilerplatr.Abstractions.Errors;

public record Error
{
    public static readonly Error None = new
    (
        code: string.Empty,
        field: string.Empty,
        description: string.Empty,
        type: ErrorType.Failure
    );
    public static readonly Error NullValue = new
    (
        code: "General.Null",
        field: "General.Null",
        description: "Null value was provided",
        type: ErrorType.Failure
    );

    public Error(string code, string field, string description, ErrorType type)
    {
        Code = code;
        Field = field;
        Description = description;
        Type = type;
    }

    public string Code { get; }
    public string Field { get; }
    public string Description { get; }
    public ErrorType Type { get; }


    ////////////////////////
    /// Predefined Error Definitions
    ////////////////////////
    public static Error Failure(string code, string field, string description) => new(code, field, description, ErrorType.Failure);
    public static Error Problem(string code, string field, string description) => new(code, field, description, ErrorType.Problem);
    public static Error NotFound(string code, string field, string description) => new(code, field, description, ErrorType.NotFound);
    public static Error Conflict(string code, string field, string description) => new(code, field, description, ErrorType.Conflict);
    public static Error Forbidden(string code, string field, string description) => new(code, field, description, ErrorType.Forbidden);
    public static Error Unauthorized(string code, string field, string description) => new(code, field, description, ErrorType.Unauthorized);
}
