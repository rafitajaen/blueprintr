namespace Boilerplatr.Extensions;

public static class DateTimeExtensions
{
    private static readonly DateTime UnixEpochStart = new(year: 1970, month: 1, day: 1, hour: 0, minute: 0, second: 0, kind: DateTimeKind.Utc);
    public static long ToUnixEpochMilliseconds(this DateTime date) => (long) date.ToUniversalTime().Subtract(UnixEpochStart).TotalMilliseconds;
    public static string ToCustomString(this DateTime date, bool includeHours = true)
    {
        var output = $"{date.Year:d4}_{date.Month:d2}_{date.Day:d2}";

        if (includeHours)
        {
            output += $" {date.Hour:d2}:{date.Minute:d2}:{date.Day:d2}";
        }

        return output;
    }
}