using System.Collections.Frozen;

namespace Boilerplatr.Themes;

public static class ThemeCodes
{
    public const string Light = "light";
    public const string Dark = "dark";

    public static readonly FrozenSet<string> All = [Light, Dark];
    public static bool IsValid(string? code) => !string.IsNullOrWhiteSpace(code) && All.Contains(code);
}
