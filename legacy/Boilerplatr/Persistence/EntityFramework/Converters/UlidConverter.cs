using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Boilerplatr.Persistence.EntityFramework.Converters;

public class UlidConverter : ValueConverter<Ulid, Guid>
{
    public UlidConverter() : base
    (
        ulid => ulid.ToGuid(),
        guid => new Ulid(guid)
    ) { }
}
