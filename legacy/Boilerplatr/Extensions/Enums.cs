namespace Boilerplatr.Extensions;

public static class Enums
{
    public static bool TryParseInt<TEnum>(string? value, bool ignoreCase, out int result) where TEnum : struct
    {
        if (Enum.TryParse<TEnum>(value, ignoreCase, out var enumResult))
        {
            result = Convert.ToInt32(enumResult);
            return true;
        }

        result = 0;
        return false;
    }
}
