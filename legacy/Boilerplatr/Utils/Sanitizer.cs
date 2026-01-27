using System.Text.RegularExpressions;
using NodaTime;

namespace Boilerplatr.Utils;

public static partial class Sanitizer
{
    private const char SimpleQuote = '\'';
    public static string Escape(this string unsafeString)
    {
        return unsafeString.Replace(SimpleQuote, ' ');
    }

    public static string Sanitize(this string unsafeString)
    {
        return unsafeString
                .ReplaceMultipleSpaces()
                .ReplaceLineBreaks()
                .ReplaceForbiddenJsonChars()
                .ReplaceEmojis()
                .ReplaceDoubleQuotes()
                .Trim();
    }

    public static string? SanitizeNullable(this string? unsafeString)
    {
        return unsafeString?
                .ReplaceMultipleSpaces()
                .ReplaceLineBreaks()
                .ReplaceForbiddenJsonChars()
                .ReplaceEmojis()
                .ReplaceDoubleQuotes()
                .Trim();
    }

    public static string SanitizeSchemaName(this string unsafeString)
    {
        return unsafeString
                .Replace(' ', '_')
                .Replace(':', '_')
                .ReplaceForbiddenSchemaChars()
                .ReplaceEmojis()
                .ReplaceDoubleQuotes()
                .Trim()
                .ToLowerInvariant();
    }

     public static Instant ParsePostedTime(string text, Instant? now = null)
    {
        // Regex para capturar número + unidad (inglés o español)
        var regex = PostedTimeRegex();
        var matches = regex.Matches(text);

        if (matches.Count == 0)
        {
            throw new ArgumentException("Could not parse the text.");
        }

        // Instant actual si no se pasa

        Instant currentInstant = now ?? SystemClock.Instance.GetCurrentInstant();

        // Convertimos a ZonedDateTime en UTC para poder manipular meses y años
        var zonedNow = currentInstant.InZone(DateTimeZone.Utc);
        Duration totalDuration = Duration.Zero;

        foreach (Match match in matches)
        {
            int amount = int.Parse(match.Groups[1].Value);
            string unit = match.Groups[2].Value.ToLower();

            switch (unit)
            {
                case "minute":
                case "minutes":
                case "minuto":
                case "minutos":
                    totalDuration += Duration.FromMinutes(amount);
                    break;

                case "hour":
                case "hours":
                case "hora":
                case "horas":
                    totalDuration += Duration.FromHours(amount);
                    break;

                case "day":
                case "days":
                case "dia":
                case "dias":
                case "día":
                case "días":
                    totalDuration += Duration.FromDays(amount);
                    break;

                case "week":
                case "weeks":
                case "semana":
                case "semanas":
                    totalDuration += Duration.FromDays(amount * 7);
                    break;

                case "month":
                case "months":
                case "mes":
                case "meses":
                    // ZonedDateTime → LocalDateTime → plus months → ZonedDateTime
                    var localMonth = zonedNow.LocalDateTime.PlusMonths(-amount);
                    zonedNow = localMonth.InZoneStrictly(DateTimeZone.Utc);
                    break;

                case "year":
                case "years":
                case "año":
                case "años":
                    var localYear = zonedNow.LocalDateTime.PlusYears(-amount);
                    zonedNow = localYear.InZoneStrictly(DateTimeZone.Utc);
                    break;

                default:
                    throw new ArgumentException($"Unrecognized time unit: {unit}");
            }
        }

        // Restamos las unidades fijas (minutes, hours, days, weeks)
        return zonedNow.ToInstant() - totalDuration;
    }

    /// <summary>
    /// Convierte espacios consecutivos en uno solo
    /// </summary>
    public static string ReplaceMultipleSpaces(this string unsafeString) => MultipleSpacesRegex().Replace(unsafeString, " ");

    /// <summary>
    /// Elimina los saltos de línea múltiples
    /// </summary>
    public static string ReplaceLineBreaks(this string unsafeString) => LineBreaksRegex().Replace(unsafeString, Environment.NewLine);

    /// <summary>
    /// Elimina los caracteres "'`{}[]$~
    /// </summary>
    public static string ReplaceForbiddenJsonChars(this string unsafeString) => ForbbidenJsonCharsRegex().Replace(unsafeString, "");

    /// <summary>
    /// Elimina emojis (cubre varios rangos Unicode)
    /// </summary>
    public static string ReplaceEmojis(this string unsafeString) => EmojisRegex().Replace(unsafeString, "");

    /// <summary>
    /// Elimina comillas dobles
    /// </summary>
    public static string ReplaceDoubleQuotes(this string unsafeString) => DoubleQuotesRegex().Replace(unsafeString, "");

    /// <summary>
    /// Elimina caracteres extraños
    /// </summary>
    public static string ReplaceForbiddenSchemaChars(this string unsafeString) => ForbbidenSchemaCharsRegex().Replace(unsafeString, "");

    [GeneratedRegex(@"\s+")]
    private static partial Regex MultipleSpacesRegex();

    [GeneratedRegex(@"(?:[\u2700-\u27bf]|(?:\ud83c[\udde6-\uddff]){2}|[\ud800-\udbff][\udc00-\udfff])[\ufe0e\ufe0f]?(?:[\u0300-\u036f\ufe20-\ufe23\u20d0-\u20f0]|\ud83c[\udffb-\udfff])?(?:\u200d(?:[^\ud800-\udfff]|(?:\ud83c[\udde6-\uddff]){2}|[\ud800-\udbff][\udc00-\udfff])[\ufe0e\ufe0f]?(?:[\u0300-\u036f\ufe20-\ufe23\u20d0-\u20f0]|\ud83c[\udffb-\udfff])?)*")]
    private static partial Regex EmojisRegex();

    [GeneratedRegex(@"['`{}\[\]\$\~]")]
    private static partial Regex ForbbidenJsonCharsRegex();

    [GeneratedRegex(@"[!@#$%^&*()+=\[{\]};:<>|./?¿,-]")]
    private static partial Regex ForbbidenSchemaCharsRegex();

    [GeneratedRegex(@"""")]
    private static partial Regex DoubleQuotesRegex();

    [GeneratedRegex(@"(\r\n|\r|\n)+")]
    private static partial Regex LineBreaksRegex();

    [GeneratedRegex(@"(\d+)\s*(minute|minutes|minuto|minutos|hour|hours|hora|horas|day|days|dia|dias|día|días|week|weeks|semana|semanas|month|months|mes|meses|year|years|año|años)", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex PostedTimeRegex();
}
