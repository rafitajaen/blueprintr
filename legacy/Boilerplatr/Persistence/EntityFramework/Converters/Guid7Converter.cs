using Boilerplatr.Shared;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Boilerplatr.Persistence.EntityFramework.Converters;

public class Guid7Converter : ValueConverter<Guid7, Guid>
{
    public Guid7Converter() : base
    (
        guid7 => guid7.Value,
        guid => Guid7.FromGuid(guid)
    ) { }
}
