using Boilerplatr.Utils;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Boilerplatr.Shared;

public class Slug(string value) : IEquatable<Slug>
{
    public string Value { get; set; } = StringUtilities.KebabizeUrl($"/{value}");

    public override string ToString() => Value;

    public bool Equals(Slug? other)
    {
        if (other is null)
        {
            return false;
        }

        return string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj)
    {
        return obj is Slug other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Value?.ToLowerInvariant().GetHashCode() ?? 0;
    }

    public static bool operator ==(Slug? left, Slug? right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return left.Equals(right);
    }

    public static bool operator !=(Slug? left, Slug? right) => !(left == right);
}


public class SlugConverter : ValueConverter<Slug, string>
{
    public SlugConverter() : base
    (
        slug => slug.Value,
        s => new Slug(s)
    ) { }
}
