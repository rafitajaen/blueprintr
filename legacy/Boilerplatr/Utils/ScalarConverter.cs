namespace Boilerplatr.Utils;

/// <summary>
/// Collection of utility methods to convert database scalar returns objects into primitive types.
/// </summary>
public static class ScalarConverter
{
    private static bool IsDbNull(this object? o) => o is null || o == DBNull.Value;

    /// <summary>
    /// Converts a database scalar return object into bool primitive type.
    /// </summary>
    public static bool ToBoolean(object? o) => !o.IsDbNull() && Convert.ToBoolean(o);

    /// <summary>
    /// Converts a database scalar return object into Int32 primitive type.
    /// </summary>
    public static int? ToInteger(object? o) => o.IsDbNull() ? null : Convert.ToInt32(o);
}