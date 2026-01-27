using NodaTime;

namespace Boilerplatr.Utils;

public record TimeAgoResult(int Value, PeriodUnits Unit)
{
    public override string ToString()
    {
        var unit = Unit switch
        {
            PeriodUnits.Years => "yr",
            PeriodUnits.Months => "mo",
            PeriodUnits.Days => "d",
            PeriodUnits.Hours => "h",
            PeriodUnits.Minutes => "m",
            PeriodUnits.Seconds => "s",
            _ => string.Empty
        };

        return $"{Value} {unit}";
    }
};

public static class TimeAgo
{
    public static TimeAgoResult Since
    (
        this Instant start,
        Instant end,
        PeriodUnits units = PeriodUnits.Years | PeriodUnits.Months | PeriodUnits.Days | PeriodUnits.Hours | PeriodUnits.Minutes | PeriodUnits.Seconds
    )
    {
        var period = Period.Between
        (
            start: start.InUtc().LocalDateTime,
            end: end.InUtc().LocalDateTime,
            units: units
        );

        if (period.Years > 0)
        {
            return new TimeAgoResult(period.Years, PeriodUnits.Years);
        }

        if (period.Months > 0)
        {
            return new TimeAgoResult(period.Months, PeriodUnits.Months);
        }

        if (period.Days > 0)
        {
            return new TimeAgoResult(period.Days, PeriodUnits.Days);
        }

        if (period.Hours > 0)
        {
            return new TimeAgoResult((int)period.Hours, PeriodUnits.Hours);
        }

        if (period.Minutes > 0)
        {
            return new TimeAgoResult((int)period.Minutes, PeriodUnits.Minutes);
        }

        return new TimeAgoResult((int)period.Seconds, PeriodUnits.Seconds);
    }
}
