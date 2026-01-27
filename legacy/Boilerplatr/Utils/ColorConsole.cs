namespace Boilerplatr.Utils;

/// <summary>
/// Write Colored Console Messages.
/// </summary>
/// 
/// <remarks>
/// Added in version 0.0.1
/// </remarks>
public static class ColorConsole
{
    /// <summary>
    /// Writes a colored message to the console, restoring the previous color.
    /// </summary>
    /// 
    /// <param name="value">
    /// String that represents the message to be written on the console.
    /// </param>
    /// 
    /// <param name="foregroundColor">
    /// ForegroundColor that will have the message in console.
    /// </param>
    public static void WriteLine(string value, ConsoleColor foregroundColor)
    {
        var previous = Console.ForegroundColor;

        Console.ForegroundColor = foregroundColor;
        Console.WriteLine(value);
        Console.ForegroundColor = previous;
    }

    public static void Write(string value, ConsoleColor foregroundColor)
    {
        var previous = Console.ForegroundColor;

        Console.ForegroundColor = foregroundColor;
        Console.Write(value);
        Console.ForegroundColor = previous;
    }

    public static void WriteKeyValueLine(string key, string? value, ConsoleColor keyForegroundColor = ConsoleColor.DarkGreen, ConsoleColor valueForegroundColor = ConsoleColor.DarkGray)
    {
        if (value is not null)
        {
            Write($"{key} ", keyForegroundColor);
            WriteLine(value, valueForegroundColor);
        }
    }
}
