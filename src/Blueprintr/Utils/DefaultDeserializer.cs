using System.Text.Json;
using System.Text.Json.Serialization;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;

namespace Blueprintr.Utils;

/// <summary>
/// Provides default JSON deserialization options for the application.
/// </summary>
/// <remarks>
/// This class configures JSON deserialization with lenient parsing rules, camelCase naming,
/// and NodaTime support. These settings allow for flexible input while maintaining consistency
/// with serialization output.
/// Added in version 1.0.0.
/// </remarks>
public static class DefaultDeserializer
{
    /// <summary>
    /// Gets the default JSON deserializer options configured for the application.
    /// </summary>
    /// <value>
    /// A <see cref="JsonSerializerOptions"/> instance configured with:
    /// <list type="bullet">
    /// <item><description>Trailing commas are allowed in JSON input</description></item>
    /// <item><description>Property name matching is case-insensitive</description></item>
    /// <item><description>Comments in JSON are skipped during parsing</description></item>
    /// <item><description>Null values are ignored when writing (WhenWritingNull)</description></item>
    /// <item><description>Property names use camelCase naming policy</description></item>
    /// <item><description>Dictionary keys use camelCase naming policy</description></item>
    /// <item><description>NodaTime types are supported via TZDB time zone provider</description></item>
    /// </list>
    /// </value>
    /// <remarks>
    /// These options are suitable for API request parsing and general JSON deserialization scenarios
    /// where lenient parsing and case-insensitive property matching improve developer experience.
    /// Added in version 1.0.0.
    /// </remarks>
    public static readonly JsonSerializerOptions Options = new JsonSerializerOptions()
    {
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
    }.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
}
