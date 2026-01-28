using Blueprintr.Utils;

namespace Blueprintr.Tests.Utils;

/// <summary>
/// Tests for <see cref="ColorConsole"/> utility class that provides colored console output.
/// </summary>
[TestFixture]
public class ColorConsoleTests
{
    private ConsoleColor _originalForegroundColor;
    private TextWriter _originalConsoleOut = null!;
    private StringWriter _consoleOutput = null!;

    [SetUp]
    public void SetUp()
    {
        _originalForegroundColor = Console.ForegroundColor;
        _originalConsoleOut = Console.Out;
        _consoleOutput = new StringWriter();
        Console.SetOut(_consoleOutput);
    }

    [TearDown]
    public void TearDown()
    {
        Console.SetOut(_originalConsoleOut);
        Console.ForegroundColor = _originalForegroundColor;
        _consoleOutput.Dispose();
    }

    [Test]
    public void WriteLine_RestoresOriginalColor()
    {
        // Arrange
        var originalColor = Console.ForegroundColor;
        var testColor = ConsoleColor.Cyan;

        // Ensure test color is different from original
        if (originalColor == testColor)
        {
            testColor = ConsoleColor.Magenta;
        }

        // Act
        ColorConsole.WriteLine("Test message", testColor);

        // Assert
        Assert.That(Console.ForegroundColor, Is.EqualTo(originalColor),
            "WriteLine should restore the original foreground color after writing");
    }

    [Test]
    public void WriteLine_WritesMessageWithNewLine()
    {
        // Arrange
        var testMessage = "Test message for WriteLine";

        // Act
        ColorConsole.WriteLine(testMessage, ConsoleColor.Green);

        // Assert
        var output = _consoleOutput.ToString();
        Assert.That(output, Does.Contain(testMessage),
            "WriteLine should write the message to console");
        Assert.That(output, Does.EndWith(Environment.NewLine),
            "WriteLine should append a new line after the message");
    }

    [Test]
    public void Write_RestoresOriginalColor()
    {
        // Arrange
        var originalColor = Console.ForegroundColor;
        var testColor = ConsoleColor.Yellow;

        // Ensure test color is different from original
        if (originalColor == testColor)
        {
            testColor = ConsoleColor.Blue;
        }

        // Act
        ColorConsole.Write("Test message", testColor);

        // Assert
        Assert.That(Console.ForegroundColor, Is.EqualTo(originalColor),
            "Write should restore the original foreground color after writing");
    }

    [Test]
    public void Write_WritesMessageWithoutNewLine()
    {
        // Arrange
        var testMessage = "Test message for Write";

        // Act
        ColorConsole.Write(testMessage, ConsoleColor.Red);

        // Assert
        var output = _consoleOutput.ToString();
        Assert.That(output, Is.EqualTo(testMessage),
            "Write should write exactly the message without adding a new line");
        Assert.That(output, Does.Not.EndWith(Environment.NewLine),
            "Write should not append a new line after the message");
    }

    [Test]
    public void WriteKeyValueLine_WithNullValue_WritesEmpty()
    {
        // Arrange
        var key = "TestKey:";
        string? value = null;

        // Act
        ColorConsole.WriteKeyValueLine(key, value);

        // Assert
        var output = _consoleOutput.ToString();
        Assert.That(output, Is.Empty,
            "WriteKeyValueLine should not write anything when value is null");
    }

    [Test]
    public void WriteKeyValueLine_FormatsKeyValueCorrectly()
    {
        // Arrange
        var key = "Configuration:";
        var value = "Production";

        // Act
        ColorConsole.WriteKeyValueLine(key, value);

        // Assert
        var output = _consoleOutput.ToString();
        Assert.That(output, Does.Contain(key),
            "WriteKeyValueLine should include the key in output");
        Assert.That(output, Does.Contain(value),
            "WriteKeyValueLine should include the value in output");
        Assert.That(output, Does.EndWith(Environment.NewLine),
            "WriteKeyValueLine should end with a new line");
    }

    [Test]
    public void WriteKeyValueLine_RestoresOriginalColor()
    {
        // Arrange
        var originalColor = Console.ForegroundColor;
        var key = "Status:";
        var value = "Active";

        // Act
        ColorConsole.WriteKeyValueLine(key, value, ConsoleColor.DarkGreen, ConsoleColor.DarkGray);

        // Assert
        Assert.That(Console.ForegroundColor, Is.EqualTo(originalColor),
            "WriteKeyValueLine should restore the original foreground color after writing");
    }

    [Test]
    public void WriteKeyValueLine_WithEmptyValue_WritesKeyAndEmptyValue()
    {
        // Arrange
        var key = "Message:";
        var value = string.Empty;

        // Act
        ColorConsole.WriteKeyValueLine(key, value);

        // Assert
        var output = _consoleOutput.ToString();
        Assert.That(output, Does.Contain(key),
            "WriteKeyValueLine should include the key even when value is empty");
        Assert.That(output, Does.EndWith(Environment.NewLine),
            "WriteKeyValueLine should end with a new line even when value is empty");
    }

    [Test]
    public void WriteKeyValueLine_UsesDefaultColors()
    {
        // Arrange
        var originalColor = Console.ForegroundColor;
        var key = "Default:";
        var value = "Colors";

        // Act
        ColorConsole.WriteKeyValueLine(key, value);

        // Assert
        // Since we can't easily check what colors were used during writing,
        // we verify the color is restored and the output is correct
        Assert.That(Console.ForegroundColor, Is.EqualTo(originalColor),
            "WriteKeyValueLine should restore color even with default color parameters");
        var output = _consoleOutput.ToString();
        Assert.That(output, Does.Contain($"{key} {value}"),
            "WriteKeyValueLine should format key and value with space separator");
    }
}
