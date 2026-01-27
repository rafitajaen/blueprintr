namespace Blueprintr.Utils;

/// <summary>
/// Provides utility methods for writing colored text to the console.
/// </summary>
/// <remarks>
/// This class helps improve console output readability by allowing different colors
/// for different types of messages while automatically restoring the previous console color.
/// Added in version 1.0.0.
/// </remarks>
public static class ColorConsole
{
    /// <summary>
    /// Writes a colored message to the console followed by a line terminator, then restores the previous color.
    /// </summary>
    /// <param name="value">The string to write to the console.</param>
    /// <param name="foregroundColor">The foreground color to use for the message.</param>
    /// <remarks>Added in version 1.0.0.</remarks>
    public static void WriteLine(string value, ConsoleColor foregroundColor)
    {
        var previous = Console.ForegroundColor;

        Console.ForegroundColor = foregroundColor;
        Console.WriteLine(value);
        Console.ForegroundColor = previous;
    }

    /// <summary>
    /// Writes a colored message to the console without a line terminator, then restores the previous color.
    /// </summary>
    /// <param name="value">The string to write to the console.</param>
    /// <param name="foregroundColor">The foreground color to use for the message.</param>
    /// <remarks>Added in version 1.0.0.</remarks>
    public static void Write(string value, ConsoleColor foregroundColor)
    {
        var previous = Console.ForegroundColor;

        Console.ForegroundColor = foregroundColor;
        Console.Write(value);
        Console.ForegroundColor = previous;
    }

    /// <summary>
    /// Writes a key-value pair to the console with customizable colors for both the key and value.
    /// </summary>
    /// <param name="key">The key portion of the key-value pair.</param>
    /// <param name="value">The value portion of the key-value pair. If null, nothing is written.</param>
    /// <param name="keyForegroundColor">The foreground color for the key. Defaults to DarkGreen.</param>
    /// <param name="valueForegroundColor">The foreground color for the value. Defaults to DarkGray.</param>
    /// <remarks>
    /// The key and value are written on the same line with a space separator.
    /// If the value is null, the entire line is skipped.
    /// Added in version 1.0.0.
    /// </remarks>
    public static void WriteKeyValueLine(string key, string? value, ConsoleColor keyForegroundColor = ConsoleColor.DarkGreen, ConsoleColor valueForegroundColor = ConsoleColor.DarkGray)
    {
        if (value is not null)
        {
            Write($"{key} ", keyForegroundColor);
            WriteLine(value, valueForegroundColor);
        }
    }
}
