using Boilerplatr.Shared;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Boilerplatr.Persistence.EntityFramework.Converters;

public class NormalizedStringConverter : ValueConverter<NormalizedString, string>
{
    public NormalizedStringConverter() : base
    (
        normalized => normalized.Value,
        s => new NormalizedString(s)
    ) { }
}
