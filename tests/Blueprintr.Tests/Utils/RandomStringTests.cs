using Blueprintr.Utils;

namespace Blueprintr.Tests.Utils;

/// <summary>
/// Tests for <see cref="RandomString"/> cryptographically secure random string generation.
/// </summary>
[TestFixture]
public class RandomStringTests
{
    [Test]
    public void Generate_WithPositiveLength_ReturnsStringOfCorrectLength()
    {
        // Arrange
        const int length = 16;

        // Act
        var result = RandomString.Generate(length);

        // Assert
        Assert.That(result, Has.Length.EqualTo(length));
    }

    [Test]
    public void Generate_WithZeroLength_ReturnsEmptyString()
    {
        // Arrange
        const int length = 0;

        // Act
        var result = RandomString.Generate(length);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void Generate_WithNegativeLength_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        const int length = -1;

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => RandomString.Generate(length));
        Assert.That(exception!.ParamName, Is.EqualTo("length"));
    }

    [Test]
    public void Generate_WithNullCharset_ThrowsArgumentNullException()
    {
        // Arrange
        const int length = 10;
        string charset = null!;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => RandomString.Generate(length, charset));
        Assert.That(exception!.ParamName, Is.EqualTo("charset"));
    }

    [Test]
    public void Generate_WithEmptyCharset_ThrowsArgumentException()
    {
        // Arrange
        const int length = 10;
        const string charset = "";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => RandomString.Generate(length, charset));
        Assert.That(exception!.ParamName, Is.EqualTo("charset"));
    }

    [Test]
    public void Generate_OutputContainsOnlyCharsetCharacters()
    {
        // Arrange
        const int length = 100;
        const string charset = "ABC123";

        // Act
        var result = RandomString.Generate(length, charset);

        // Assert
        Assert.That(result, Has.Length.EqualTo(length));
        Assert.That(result.All(c => charset.Contains(c)), Is.True,
            "All characters in the result should come from the charset");
    }

    [Test]
    public void Generate_MultipleCalls_ProducesDifferentResults()
    {
        // Arrange
        const int length = 32;
        const int iterations = 10;

        // Act
        var results = Enumerable.Range(0, iterations)
            .Select(_ => RandomString.Generate(length))
            .ToList();

        // Assert
        var uniqueResults = results.Distinct().Count();
        Assert.That(uniqueResults, Is.EqualTo(iterations),
            "Each call should produce a different random string due to cryptographic randomness");
    }

    [Test]
    public void Generate_WithCustomCharset_UsesOnlyThoseCharacters()
    {
        // Arrange
        const int length = 50;
        const string customCharset = "XYZ789";

        // Act
        var result = RandomString.Generate(length, customCharset);

        // Assert
        Assert.That(result, Has.Length.EqualTo(length));

        foreach (char c in result)
        {
            Assert.That(c.ToString(), Is.AnyOf(customCharset.ToCharArray().Select(ch => ch.ToString()).ToArray()),
                $"Character '{c}' should be from the custom charset");
        }
    }

    [Test]
    public void Generate_WithSingleCharacterCharset_ReturnsRepeatedCharacter()
    {
        // Arrange
        const int length = 20;
        const string charset = "X";

        // Act
        var result = RandomString.Generate(length, charset);

        // Assert
        Assert.That(result, Has.Length.EqualTo(length));
        Assert.That(result.All(c => c == 'X'),
            "All characters should be 'X' when charset contains only 'X'");
    }

    [Test]
    public void Generate_WithDefaultCharset_UsesAlphanumericCharacters()
    {
        // Arrange
        const int length = 100;

        // Act
        var result = RandomString.Generate(length);

        // Assert
        Assert.That(result, Has.Length.EqualTo(length));
        Assert.That(result.All(c => char.IsLetterOrDigit(c)), Is.True,
            "Default charset should produce alphanumeric characters");
    }

    [TestCase(1)]
    [TestCase(5)]
    [TestCase(10)]
    [TestCase(50)]
    [TestCase(100)]
    [TestCase(1000)]
    public void Generate_WithVariousLengths_ReturnsCorrectLength(int length)
    {
        // Act
        var result = RandomString.Generate(length);

        // Assert
        Assert.That(result, Has.Length.EqualTo(length));
    }
}
