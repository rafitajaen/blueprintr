using NodaTime;

namespace Boilerplatr.Extensions;

public static class InstantExtensions
{
    public static DateTimeZone UtcZone = DateTimeZoneProviders.Tzdb["UTC"];
    public static int GetMinute(this Instant instant)
    {
        return instant.InZone(UtcZone).LocalDateTime.Minute;
    }
}
