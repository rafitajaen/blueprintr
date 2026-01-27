using System.Security.Cryptography;
using System.Text;

namespace Boilerplatr.Utils;

/// <summary>
/// Random String Generator.
/// </summary>
/// 
/// <remarks>
/// Added in version 0.0.1
/// </remarks>
public static class RandomString
{
    /// <summary>
    /// It generates a random string of a predetermined size and using a specific CharSet.
    /// </summary>
    /// 
    /// <param name="length">
    /// Final size of the string to generate.
    /// </param>
    /// 
    /// <param name="charset">
    /// Collection of characters that will be used to generate the string.
    /// </param>
    /// 
    /// <returns>
    /// Returns a random string respecting the parameters entered.
    /// </returns>
    /// 
    /// <exception cref="ArgumentException">
    /// Thrown when charset is null or empty.
    /// </exception>
    /// 
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when length is negative.
    /// </exception>
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
