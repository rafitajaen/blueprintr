namespace Boilerplatr.Utils;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class WithSlugAttribute : Attribute
{
    public string Value { get; }

    public WithSlugAttribute(string value)
    {
        Value = value;
    }
}
