using Blueprintr.Shared;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Blueprintr.Converters;

/// <summary>
/// Converts <see cref="Guid7"/> to <see cref="Guid"/> for database storage.
/// </summary>
/// <remarks>
/// This converter allows Entity Framework Core to automatically convert between
/// the <see cref="Guid7"/> type and the database-native <see cref="Guid"/> type.
/// Added in version 1.0.0.
/// </remarks>
public class Guid7Converter : ValueConverter<Guid7, Guid>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Guid7Converter"/> class.
    /// </summary>
    /// <remarks>Added in version 1.0.0.</remarks>
    public Guid7Converter() : base
    (
        guid7 => guid7.Value,
        guid => Guid7.FromGuid(guid)
    ) { }
}
