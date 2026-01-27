using System.Diagnostics.CodeAnalysis;

namespace Boilerplatr.Shared;

public readonly struct NormalizedString : IEquatable<NormalizedString>
{
    public string Value { get; }

    public NormalizedString(string value)
    {
        Value = Normalize(value ?? throw new ArgumentNullException(nameof(value)));
    }

    public static string Normalize(string input)
    {
        return input.Trim().ToLowerInvariant();
    }

    public override string ToString() => Value;

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is NormalizedString other && Equals(other);

    public bool Equals(NormalizedString other) => Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(NormalizedString left, NormalizedString right) => left.Equals(right);
    public static bool operator !=(NormalizedString left, NormalizedString right) => !(left == right);

    // Implicit conversion from string
    public static implicit operator NormalizedString(string value) => new NormalizedString(value);

    // Implicit conversion to string
    public static implicit operator string(NormalizedString ns) => ns.Value;
}
