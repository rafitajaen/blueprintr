using System.Security.Cryptography;
using System.Text;

namespace Blueprintr.Utils;

/// <summary>
/// Provides cryptographically secure random string generation.
/// </summary>
/// <remarks>
/// This class uses <see cref="RandomNumberGenerator"/> to generate cryptographically secure random strings
/// from a specified character set. This is suitable for generating tokens, passwords, and other security-sensitive strings.
/// Added in version 1.0.0.
/// </remarks>
public static class RandomString
{
    /// <summary>
    /// Generates a cryptographically secure random string of the specified length using the given character set.
    /// </summary>
    /// <param name="length">The desired length of the generated string. Must be non-negative.</param>
    /// <param name="charset">
    /// The collection of characters to use for generating the string.
    /// Defaults to <see cref="CharSets.Alphanumeric"/> if not specified.
    /// </param>
    /// <returns>
    /// A random string of the specified length composed of characters from the charset.
    /// Returns an empty string if length is 0.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="charset"/> is null or empty.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="length"/> is negative.
    /// </exception>
    /// <remarks>
    /// This method uses <see cref="RandomNumberGenerator"/> to ensure cryptographic security,
    /// making it suitable for generating passwords, tokens, and other security-sensitive strings.
    /// Each character is independently selected with uniform probability from the charset.
    /// Added in version 1.0.0.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Generate a 16-character alphanumeric string
    /// var token = RandomString.Generate(16);
    /// 
    /// // Generate a 32-character hexadecimal string
    /// var hexToken = RandomString.Generate(32, CharSets.Hexadecimal);
    /// </code>
    /// </example>
    public static string Generate(int length, string charset = CharSets.Alphanumeric)
    {
        ArgumentException.ThrowIfNullOrEmpty(charset, nameof(charset));
        ArgumentOutOfRangeException.ThrowIfNegative(length, nameof(length));

        if (length == 0)
        {
            return string.Empty;
        }

        var randomString = new StringBuilder();

        for (var index = 0; index < length; index++)
        {
            var next = RandomNumberGenerator.GetInt32(charset.Length);
            randomString.Append(charset[next]);
        }

        return randomString.ToString();
    }
}
