using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Blueprintr.Converters;

/// <summary>
/// Converts <see cref="Ulid"/> to <see cref="Guid"/> for database storage.
/// </summary>
/// <remarks>
/// This converter allows Entity Framework Core to automatically convert between
/// the <see cref="Ulid"/> type and the database-native <see cref="Guid"/> type.
/// ULIDs are lexicographically sortable and can be efficiently stored as GUIDs.
/// Added in version 1.0.0.
/// </remarks>
public class UlidConverter : ValueConverter<Ulid, Guid>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UlidConverter"/> class.
    /// </summary>
    /// <remarks>Added in version 1.0.0.</remarks>
    public UlidConverter() : base
    (
        ulid => ulid.ToGuid(),
        guid => new Ulid(guid)
    ) { }
}
