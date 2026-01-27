namespace Blueprintr.Utils;

/// <summary>
/// Provides a collection of commonly used character sets for validation, generation, and parsing operations.
/// </summary>
/// <remarks>
/// This class contains predefined character sets for various encoding systems and formats,
/// useful for password generation, input validation, and string manipulation.
/// Added in version 1.0.0.
/// </remarks>
public static class CharSets
{
    private const string numbers = "0123456789";
    private const string charsLower = "abcdefghijklmnopqrstuvwxyz";
    private const string charsUpper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    /// <summary>
    /// Binary character set containing only '0' and '1'.
    /// </summary>
    /// <remarks>Added in version 1.0.0.</remarks>
    public const string Binary = "01";

    /// <summary>
    /// Hexadecimal character set containing digits 0-9 and lowercase letters a-f.
    /// </summary>
    /// <remarks>Added in version 1.0.0.</remarks>
    public const string Hexadecimal = numbers + "abcdef";

    /// <summary>
    /// Numeric character set containing digits 0-9.
    /// </summary>
    /// <remarks>Added in version 1.0.0.</remarks>
    public const string Numeric = numbers;

    /// <summary>
    /// Octal character set containing digits 0-7.
    /// </summary>
    /// <remarks>Added in version 1.0.0.</remarks>
    public const string Octal = "01234567";

    /// <summary>
    /// Alphabetic character set containing both lowercase (a-z) and uppercase (A-Z) letters.
    /// </summary>
    /// <remarks>Added in version 1.0.0.</remarks>
    public const string Alphabetic = charsLower + charsUpper;

    /// <summary>
    /// Lowercase alphabetic character set containing only lowercase letters (a-z).
    /// </summary>
    /// <remarks>Added in version 1.0.0.</remarks>
    public const string LowerAlphabetic = charsLower;

    /// <summary>
    /// Uppercase alphabetic character set containing only uppercase letters (A-Z).
    /// </summary>
    /// <remarks>Added in version 1.0.0.</remarks>
    public const string UpperAlphabetic = charsUpper;

    /// <summary>
    /// Alphanumeric character set containing digits (0-9), lowercase letters (a-z), and uppercase letters (A-Z).
    /// </summary>
    /// <remarks>Added in version 1.0.0.</remarks>
    public const string Alphanumeric = numbers + charsLower + charsUpper;

    /// <summary>
    /// Lowercase alphanumeric character set containing digits (0-9) and lowercase letters (a-z).
    /// </summary>
    /// <remarks>Added in version 1.0.0.</remarks>
    public const string LowerAlphanumeric = numbers + charsLower;

    /// <summary>
    /// Uppercase alphanumeric character set containing digits (0-9) and uppercase letters (A-Z).
    /// </summary>
    /// <remarks>Added in version 1.0.0.</remarks>
    public const string UpperAlphanumeric = numbers + charsUpper;
}
