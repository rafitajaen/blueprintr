namespace Boilerplatr.Extensions;

public static class TypeExtensions
{
    public static string GetNestedName(this Type type, string? childName = null)
    {
        if (type.DeclaringType is Type declaringType)
        {
            if (string.IsNullOrWhiteSpace(childName))
            {
                return GetNestedName(declaringType, type.Name);
            }

            return GetNestedName(declaringType, $"{type.Name}.{childName}");
        }

        if (string.IsNullOrWhiteSpace(childName))
        {
            return type.Name;
        }

        return $"{type.Name}.{childName}";
    }
}
