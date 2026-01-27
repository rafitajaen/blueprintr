using System.Diagnostics.CodeAnalysis;

namespace Boilerplatr.Shared;

public readonly struct MarkdownText : IEquatable<MarkdownText>
{
    public string Value { get; }

    public MarkdownText(string? value = null)
    {
        Value = value ?? string.Empty;
    }

    public static MarkdownText Empty() => new(string.Empty);

    public override string ToString() => Value;

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is MarkdownText other && Equals(other);

    public bool Equals(MarkdownText other) => Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(MarkdownText left, MarkdownText right) => left.Equals(right);
    public static bool operator !=(MarkdownText left, MarkdownText right) => !(left == right);

    // Implicit conversion from string
    public static implicit operator MarkdownText(string? value) => new(value);

    // Implicit conversion to string
    public static implicit operator string(MarkdownText ns) => ns.Value;
}
