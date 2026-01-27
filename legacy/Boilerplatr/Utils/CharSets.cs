namespace Boilerplatr.Utils;

/// <summary>
/// A collection of charsets.
/// </summary>
/// 
/// <remarks>
/// Added in version 0.0.1
/// </remarks>
public static class CharSets
{
    private const string numbers = "0123456789";
    private const string charsLower = "abcdefghijklmnopqrstuvwxyz";
    private const string charsUpper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    /// <summary>
    /// Binary Charset.
    /// </summary>
    public const string Binary = "01";

    /// <summary>
    /// Haxadecimal Charset.
    /// </summary>
    public const string Hexadecimal = numbers + "abcdef";

    /// <summary>
    /// Numeric Charset.
    /// </summary>
    public const string Numeric = numbers;

    /// <summary>
    /// Octal Charset.
    /// </summary>
    public const string Octal = "01234567";

    /// <summary>
    /// Alphabetic Charset (include lower and upper alpabetic chars).
    /// </summary>
    public const string Alphabetic = charsLower + charsUpper;

    /// <summary>
    /// Lower-Only Alphabetic Charset.
    /// </summary>
    public const string LowerAlphabetic = charsLower;

    /// <summary>
    /// Upper-Only Alphabetic Charset.
    /// </summary>
    public const string UpperAlphabetic = charsUpper;

    /// <summary>
    /// Alphanumeric Charset (include decimal numbers, lower and upper alpabetic chars).
    /// </summary>
    public const string Alphanumeric = numbers + charsLower + charsUpper;

    /// <summary>
    /// Lower-Only Alphanumeric Charset.
    /// </summary>
    public const string LowerAlphanumeric = numbers + charsLower;

    /// <summary>
    /// Upper-Only Alphanumeric Charset.
    /// </summary>
    public const string UpperAlphanumeric = numbers + charsUpper;
}