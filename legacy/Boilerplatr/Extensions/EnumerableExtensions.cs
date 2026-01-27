namespace Boilerplatr.Extensions;

public static class EnumerableExtensions
{
    public static bool ContainsAnyIn<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> value) => source.Any(x => value.Contains(x));
    public static bool ContainsAllIn<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> value) => source.All(x => value.Contains(x));

    public static IEnumerable<string> ToNameArray<TEnum>(this IEnumerable<TEnum> values) where TEnum : struct, Enum
    {
        return values?.Distinct().Select(v => v.ToString()) ?? [];
    }
    
}
