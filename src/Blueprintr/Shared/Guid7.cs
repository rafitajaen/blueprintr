using System.Diagnostics.CodeAnalysis;

namespace Blueprintr.Shared;

/// <summary>
/// Represents a GUID wrapper structure with additional functionality for version 7 UUIDs.
/// </summary>
/// <remarks>
/// Version 7 UUIDs are time-ordered and provide better database indexing performance
/// compared to random UUIDs (version 4). This struct provides a type-safe wrapper
/// around the standard <see cref="Guid"/> type with built-in support for version 7 UUIDs.
/// Added in version 1.0.0.
/// </remarks>
public readonly struct Guid7 : IEquatable<Guid7>
{
    /// <summary>
    /// Gets the underlying Guid value.
    /// </summary>
    /// <remarks>Added in version 1.0.0.</remarks>
    public required readonly Guid Value { get; init; }

    /// <summary>
    /// Creates a new Guid7 with a new version 7 GUID.
    /// </summary>
    /// <returns>A new Guid7 instance with a time-ordered version 7 GUID.</returns>
    /// <remarks>
    /// Version 7 UUIDs include a timestamp component, making them naturally sortable
    /// and improving database index performance.
    /// Added in version 1.0.0.
    /// </remarks>
    public static Guid7 NewGuid() => new() { Value = Guid.CreateVersion7() };

    /// <summary>
    /// Creates a Guid7 from an existing Guid.
    /// </summary>
    /// <param name="guid">The Guid to convert.</param>
    /// <returns>A new Guid7 instance wrapping the provided Guid.</returns>
    /// <remarks>Added in version 1.0.0.</remarks>
    public static Guid7 FromGuid(Guid guid) => new() { Value = guid };

    /// <summary>
    /// Creates a Guid7 from a string representation of a Guid.
    /// </summary>
    /// <param name="guid">The string representation of the Guid.</param>
    /// <returns>A new Guid7 instance wrapping the parsed Guid.</returns>
    /// <exception cref="FormatException">Thrown when the string is not in a valid Guid format.</exception>
    /// <remarks>Added in version 1.0.0.</remarks>
    public static Guid7 FromGuid(string guid) => new() { Value = Guid.Parse(guid) };

    /// <summary>
    /// Creates a new Guid7 with a version 7 GUID based on the specified timestamp.
    /// </summary>
    /// <param name="timestamp">The timestamp to use for the version 7 GUID.</param>
    /// <remarks>Added in version 1.0.0.</remarks>
    public static Guid7 NewGuid(DateTimeOffset timestamp) => new() { Value = Guid.CreateVersion7(timestamp) };

    /// <summary>
    /// Tries to parse a string into a Guid7.
    /// </summary>
    /// <param name="input">The string to parse.</param>
    /// <param name="guid">When this method returns, contains the parsed Guid7 if the conversion succeeded, or default if it failed.</param>
    /// <returns>true if the parse operation was successful; otherwise, false.</returns>
    /// <remarks>Added in version 1.0.0.</remarks>
    public static bool TryParse(string? input, [NotNullWhen(true)] out Guid7 guid)
    {
        guid = default;

        if (Guid.TryParse(input, out var result))
        {
            guid = FromGuid(result);
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public override string ToString() => Value.ToString();

    /// <inheritdoc />
    public override int GetHashCode() => Value.GetHashCode();

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? o) => o is Guid7 g && Value.Equals(g.Value);

    /// <inheritdoc />
    public bool Equals(Guid7 g) => Value.Equals(g.Value);

    /// <summary>
    /// Determines whether two specified instances of Guid7 are equal.
    /// </summary>
    /// <remarks>Added in version 1.0.0.</remarks>
    public static bool operator ==(Guid7 left, Guid7 right) => left.Value.Equals(right.Value);

    /// <summary>
    /// Determines whether two specified instances of Guid7 are not equal.
    /// </summary>
    /// <remarks>Added in version 1.0.0.</remarks>
    public static bool operator !=(Guid7 left, Guid7 right) => !(left.Value == right.Value);

    /// <summary>
    /// Parses a string into a Guid7.
    /// </summary>
    /// <param name="input">The string to parse.</param>
    /// <returns>A new Guid7 instance wrapping the parsed Guid.</returns>
    /// <exception cref="FormatException">Thrown when the string is not in a valid Guid format.</exception>
    /// <remarks>Added in version 1.0.0.</remarks>
    public static Guid7 Parse(string input) => FromGuid(Guid.Parse(input));
}
