using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace Boilerplatr.Extensions;

public static partial class HttpRequestExtensions
{
    public static Dictionary<string, double> ExtractLanguageWeights(this HttpRequest request)
    {
        var weightsByLanguage = new Dictionary<string, double>();

        if (request.Headers.TryGetValue(HeaderNames.AcceptLanguage, out var header))
        {
            foreach (Match match in AcceptLanguageHeaderRegex().Matches(header.ToString()))
            {
                if (match.Success && match.Groups.TryGetValue("language", out var languageGroup) && !string.IsNullOrWhiteSpace(languageGroup.Value))
                {
                    if (!match.Groups.TryGetValue("weight", out var weightGroup) || !double.TryParse(weightGroup.Value, out double weight))
                    {
                        weight = 1.0; // Fallback if parsing fails
                    }

                    weightsByLanguage[languageGroup.Value.Trim()] = weight;
                }
            }
        }

        return weightsByLanguage.OrderByDescending(kvp => kvp.Value).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    [GeneratedRegex(@"(?<language>[a-z]{1,8}(?:-[a-zA-Z]{1,8})*)(?:;q=(?<weight>0(?:\.\d{1,3})?|1(?:.0{1,3})?))?")]
    private static partial Regex AcceptLanguageHeaderRegex();
}