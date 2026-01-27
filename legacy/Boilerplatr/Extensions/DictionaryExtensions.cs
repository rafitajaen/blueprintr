namespace Boilerplatr.Extensions;

public static class DictionaryExtensions
{
    public static string GetStringOrEmpty(this Dictionary<string, object> dictionary, string key)
    {
        dictionary.TryGetValue(key, out var value);
        return value?.ToString() ?? string.Empty;
    }
}