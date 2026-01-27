using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Boilerplatr.Persistence.EntityFramework.Converters;

public class JsonConverter<T> : ValueConverter<T, string>
{
    public JsonConverter() : base
    (
        o => JsonSerializer.Serialize(o, JsonSerializerOptions.Web),
        s => JsonSerializer.Deserialize<T>(s, JsonSerializerOptions.Web)
    ) { }
}
