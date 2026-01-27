using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;

namespace Boilerplatr.Extensions;

public static class JsonExtensions
{
    public static bool TryParseJsonObject(this string json, out JsonObject? o)
    {
        try
        {
            o = JsonNode.Parse(json)?.AsObject();
        }
        catch (Exception)
        {
            o = null;
        }

        return o is not null;
    }

    public static bool TryParseJsonArray(this string json, out JsonArray? o)
    {
        try
        {
            o = JsonNode.Parse(json)?.AsArray();
        }
        catch (Exception)
        {
            o = null;
        }

        return o is not null;
    }

    public static JsonSerializerOptions DefaultSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        .ConfigureForNodaTime(DateTimeZoneProviders.Tzdb)
        .ConfigureEnums();

    private static JsonSerializerOptions ConfigureEnums(this JsonSerializerOptions options)
    {
        // Esto permite que los enums se deserialicen desde strings
        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        return options;
    }
    public static JsonSerializerOptions SnakeCase = new JsonSerializerOptions(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower,
        WriteIndented = true
    }.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

    public static T DeserializeJson<T>(this string json, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Deserialize<T>(json, options ?? DefaultSerializerOptions) ?? throw new InvalidCastException("Json deserialization result is null");
    }

    public static string SerializeToJson(this object o, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Serialize(o, options ?? DefaultSerializerOptions);
    }
}
