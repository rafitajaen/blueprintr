using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Boilerplatr.Persistence.EntityFramework.Converters;

public class TimeSpanConverter : ValueConverter<TimeSpan, string>
{
    public TimeSpanConverter() : base
    (
        timespan => timespan.ToString(@"hh\:mm\:ss"), // TimeSpan → string
        s => TimeSpan.Parse(s)
    ) { }
}
