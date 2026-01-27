namespace Boilerplatr.Utils;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class DescriptorAttribute : Attribute
{
    public string Value { get; }

    public DescriptorAttribute(string value)
    {
        Value = value;
    }
}
