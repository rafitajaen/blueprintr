using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Humanizer;

namespace Boilerplatr.Utils;

/// <summary>
/// Collection of Utilities for string.
/// </summary>
public static partial class StringUtilities
{
    public static string? RemoveAccents(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return text;
        }

        // Normalizar a FormD (descomponer acentos)
        var normalized = text.Normalize(NormalizationForm.FormD);

        // Filtrar solo los caracteres que no son marcas de acento
        var sb = new StringBuilder();
        foreach (var c in normalized)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }

        // Normalizar de nuevo a FormC
        return sb.ToString().Normalize(NormalizationForm.FormC);
    }
    
    public static string RemoveWwwPrefix(this string url)
    {
        if (url.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
        {
            return url[4..];
        }
        return url;
    }

    public static int ExtractNumbersFrom(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return 0;
        }

        // Patrón regex:
        // [^0-9] o \D -> coincide con CUALQUIER CARÁCTER que NO sea un dígito.
        // Replace sustituye todos esos caracteres no-dígito por una cadena vacía ("").
        string onlyDigits = NotNumbersRegex().Replace(input, "");

        if (string.IsNullOrEmpty(onlyDigits))
        {
            return 0;
        }

        // Intentar convertir la cadena resultante (ej: "238911") a un número largo (long).
        // Usamos 'long' en lugar de 'int' para evitar desbordamiento si la cadena de dígitos es muy larga.
        if (int.TryParse(onlyDigits, out int output))
        {
            return output;
        }

        // Devolver 0 en caso de que la conversión falle (aunque es improbable aquí)
        return 0;
    }

    public static string ExtractInitials(this string value, int max = 2)
    {
        if (string.IsNullOrWhiteSpace(value) || max <= 0)
        {
            return string.Empty;
        }

        var separators = new HashSet<char> { ' ', '-', '_', '.', ',', '/', '\\', '|', ':' };
        var initials = new List<char>(max);

        bool atWordStart = true;

        foreach (var c in value.Trim())
        {
            if (separators.Contains(c))
            {
                atWordStart = true;
                continue;
            }

            if (atWordStart && !char.IsWhiteSpace(c))
            {
                initials.Add(char.ToUpperInvariant(c));
                if (initials.Count == max)
                {
                    break;
                }

                atWordStart = false;
            }
        }

        return new string([.. initials]);
    }

    public static string Normalize(this string value)
    {
        return value.Trim().ToLowerInvariant();
    }

    public static string NormalizeUrl(this string input)
    {
        return MultipleSlashesRegex().Replace(input.Normalize(), "/");
    }

    public static string Kebabize(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        return NonAlphaNumericRegex().Replace(input.Humanize(LetterCasing.Title), "").Kebaberize();
    }

    public static string KebabizeUrl(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        return NonUrlRegex().Replace(input, "").Kebaberize().NormalizeUrl();
    }


    /// <summary>
    /// Returns true if value contains only alphanumeric characters.
    /// </summary>
    public static bool IsAlphanumeric(string value) => AlphanumericRegex().IsMatch(value);

    /// <summary>
    /// Returns true if value contains only alphabetic characters.
    /// </summary>
    public static bool IsAlphabetic(string value) => AlphabeticRegex().IsMatch(value);

    /// <summary>
    /// Returns true if value contains only lower alphabetic characters.
    /// </summary>
    public static bool IsLowerAlphabetic(string value) => LowerAlphabeticRegex().IsMatch(value);

    /// <summary>
    /// Returns true if value contains upper lower alphabetic characters.
    /// </summary>
    public static bool IsUpperAlphabetic(string value) => UpperAlphabeticRegex().IsMatch(value);

    /// <summary>
    /// Returns true if value contains only numeric characters.
    /// </summary>
    public static bool IsNumeric(string value) => NumericRegex().IsMatch(value);

    /// <summary>
    /// Returns true if value contains only binary characters.
    /// </summary>
    public static bool IsBinary(string value) => BinaryRegex().IsMatch(value);

    /// <summary>
    /// Returns true if value contains only octal characters.
    /// </summary>
    public static bool IsOctal(string value) => OctalRegex().IsMatch(value);

    /// <summary>
    /// Returns true if value contains only hexadecimal characters.
    /// </summary>
    public static bool IsHexadecimal(string value) => HexadecimalRegex().IsMatch(value);

    /// <summary>
    /// Returns true if value contains only lower alphanumeric characters.
    /// </summary>
    public static bool IsLowerAlphanumeric(string value) => LowerAlphanumericRegex().IsMatch(value);

    /// <summary>
    /// Returns true if value contains only upper alphanumeric characters.
    /// </summary>
    public static bool IsUpperAlphanumeric(string value) => UpperAlphanumericRegex().IsMatch(value);

    /// <summary>
    /// Returns true if value is a valid email.
    /// </summary>
    public static bool IsEmail(string value) => EmailRegex().IsMatch(value);

    [GeneratedRegex(@"^[a-zA-Z0-9]+$")]
    private static partial Regex AlphanumericRegex();

    [GeneratedRegex(@"^[0-9]+$")]
    private static partial Regex NumericRegex();

    [GeneratedRegex(@"^[a-zA-Z]+$")]
    private static partial Regex AlphabeticRegex();

    [GeneratedRegex(@"^[a-z]+$")]
    private static partial Regex LowerAlphabeticRegex();

    [GeneratedRegex(@"^[A-Z]+$")]
    private static partial Regex UpperAlphabeticRegex();

    [GeneratedRegex(@"^[0-1]+$")]
    private static partial Regex BinaryRegex();

    [GeneratedRegex(@"^[0-7]+$")]
    private static partial Regex OctalRegex();

    [GeneratedRegex(@"^[a-fA-F0-9]+$")]
    private static partial Regex HexadecimalRegex();

    [GeneratedRegex(@"^[a-z0-9]+$")]
    private static partial Regex LowerAlphanumericRegex();

    [GeneratedRegex(@"^[A-Z0-9]+$")]
    private static partial Regex UpperAlphanumericRegex();

    [GeneratedRegex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")]
    private static partial Regex EmailRegex();

    [GeneratedRegex(@"(?<!:)/{2,}")]
    private static partial Regex MultipleSlashesRegex();

    [GeneratedRegex("[^a-zA-Z0-9]")]
    private static partial Regex NonAlphaNumericRegex();

    [GeneratedRegex("[^a-zA-Z0-9/-]")]
    private static partial Regex NonUrlRegex();

    [GeneratedRegex(@"[^\r\n]+")]
    public static partial Regex LineRegex();

    [GeneratedRegex(@"[^\d]")]
    private static partial Regex NotNumbersRegex();
}
