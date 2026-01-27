using System.Diagnostics.CodeAnalysis;

namespace Boilerplatr.Shared;

public readonly struct Guid7 : IEquatable<Guid7>
{
    public required readonly Guid Value { get; init; }

    public static Guid7 NewGuid() => new() { Value = Guid.CreateVersion7() };
    public static Guid7 FromGuid(Guid guid) => new() { Value = guid };
    public static Guid7 FromGuid(string guid) => new() { Value = Guid.Parse(guid) };
    public static Guid7 NewGuid(DateTimeOffset timestamp) => new() { Value = Guid.CreateVersion7(timestamp) };

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
    public override string ToString() => Value.ToString();
    public override int GetHashCode() => Value.GetHashCode();
    public override bool Equals([NotNullWhen(true)] object? o) => o is Guid7 g && Value.Equals(g.Value);
    public bool Equals(Guid7 g) => Value.Equals(g.Value);

    public static bool operator ==(Guid7 left, Guid7 right) => left.Value.Equals(right.Value);

    public static bool operator !=(Guid7 left, Guid7 right) => !(left.Value == right.Value);

    public static Guid7 Parse(string input) => FromGuid(Guid.Parse(input));
}
