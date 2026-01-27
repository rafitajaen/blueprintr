namespace Boilerplatr.Utils;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class HumanNameAttribute : Attribute
{
    public string Value { get; }

    public HumanNameAttribute(string value)
    {
        Value = value;
    }
}
