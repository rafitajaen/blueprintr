using System.Collections.Frozen;

namespace Boilerplatr.Utils;

public static class LanguageCodes
{
    public const string English = "en";
    public const string Spanish = "es";
    public const string Portuguese = "pt";
    public const string French = "fr";
    public const string German = "de";
    public const string Italian = "it";

    public static readonly FrozenSet<string> All = [English, Spanish, Portuguese, French, German, Italian];

    public static bool IsValid(string code) => !string.IsNullOrWhiteSpace(code) && All.Contains(code);
}
