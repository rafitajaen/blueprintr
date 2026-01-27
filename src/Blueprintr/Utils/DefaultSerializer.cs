using System.Text.Json;
using System.Text.Json.Serialization;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;

namespace Blueprintr.Utils;

/// <summary>
/// Provides default JSON serialization options for the application.
/// </summary>
/// <remarks>
/// This class configures JSON serialization with camelCase naming, null value handling,
/// and NodaTime support for date/time types. These settings ensure consistent JSON output
/// across the application.
/// Added in version 1.0.0.
/// </remarks>
public static class DefaultSerializer
{
    /// <summary>
    /// Gets the default JSON serializer options configured for the application.
    /// </summary>
    /// <value>
    /// A <see cref="JsonSerializerOptions"/> instance configured with:
    /// <list type="bullet">
    /// <item><description>Null values are ignored when writing (WhenWritingNull)</description></item>
    /// <item><description>Property names use camelCase naming policy</description></item>
    /// <item><description>Dictionary keys use camelCase naming policy</description></item>
    /// <item><description>NodaTime types are supported via TZDB time zone provider</description></item>
    /// </list>
    /// </value>
    /// <remarks>
    /// These options are suitable for API responses and general JSON serialization scenarios
    /// where consistent formatting and null handling are desired.
    /// Added in version 1.0.0.
    /// </remarks>
    public static readonly JsonSerializerOptions Options = new JsonSerializerOptions()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
    }.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
}
