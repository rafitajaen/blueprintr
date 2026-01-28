using Blueprintr.Shared;
using NUnit.Framework;

namespace Blueprintr.Tests.Shared;

[TestFixture]
public class Guid7Tests
{
    [Test]
    public void NewGuid_ShouldCreateNonEmptyGuid()
    {
        var guid7 = Guid7.NewGuid();
        Assert.That(guid7.Value, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public void FromGuid_ShouldStoreGuid()
    {
        var guid = Guid.NewGuid();
        var guid7 = Guid7.FromGuid(guid);
        Assert.That(guid7.Value, Is.EqualTo(guid));
    }

    [Test]
    public void FromString_ShouldParseGuid()
    {
        var guid = Guid.NewGuid();
        var guidString = guid.ToString();
        var guid7 = Guid7.FromGuid(guidString);
        Assert.That(guid7.Value, Is.EqualTo(guid));
    }

    [Test]
    public void TryParse_ValidInput_ShouldReturnTrueAndGuid()
    {
        var guid = Guid.NewGuid();
        var result = Guid7.TryParse(guid.ToString(), out var guid7);
        
        Assert.That(result, Is.True);
        Assert.That(guid7.Value, Is.EqualTo(guid));
    }

    [Test]
    public void TryParse_InvalidInput_ShouldReturnFalse()
    {
        var result = Guid7.TryParse("invalid-guid", out var guid7);
        
        Assert.That(result, Is.False);
        Assert.That(guid7.Value, Is.EqualTo(Guid.Empty));
    }

    [Test]
    public void Equality_ShouldWork()
    {
        var guid = Guid.NewGuid();
        var guid7a = Guid7.FromGuid(guid);
        var guid7b = Guid7.FromGuid(guid);
        var guid7c = Guid7.NewGuid();

        Assert.That(guid7a, Is.EqualTo(guid7b));
        Assert.That(guid7a == guid7b, Is.True);
        Assert.That(guid7a != guid7b, Is.False);
        
        Assert.That(guid7a, Is.Not.EqualTo(guid7c));
        Assert.That(guid7a == guid7c, Is.False);
        Assert.That(guid7a != guid7c, Is.True);
    }
    
    [Test]
    public void NewGuid_WithTimestamp_ShouldCreateGuid()
    {
         var timestamp = DateTimeOffset.UtcNow;
         var guid7 = Guid7.NewGuid(timestamp);
         Assert.That(guid7.Value, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public void ToString_ReturnsCorrectFormat()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var guid7 = Guid7.FromGuid(guid);
        var expectedFormat = guid.ToString();

        // Act
        var result = guid7.ToString();

        // Assert
        Assert.That(result, Is.EqualTo(expectedFormat));
        Assert.That(result, Does.Match(@"^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$"));
    }

    [Test]
    public void GetHashCode_IsConsistent()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var guid7a = Guid7.FromGuid(guid);
        var guid7b = Guid7.FromGuid(guid);
        var guid7c = Guid7.NewGuid();

        // Act
        var hashA1 = guid7a.GetHashCode();
        var hashA2 = guid7a.GetHashCode();
        var hashB = guid7b.GetHashCode();
        var hashC = guid7c.GetHashCode();

        // Assert
        // Same instance should return same hash code (consistency)
        Assert.That(hashA1, Is.EqualTo(hashA2));

        // Equal Guid7 instances should have same hash code
        Assert.That(hashA1, Is.EqualTo(hashB));

        // Hash should match underlying Guid hash
        Assert.That(hashA1, Is.EqualTo(guid.GetHashCode()));

        // Different Guid7 should likely have different hash (not guaranteed, but very likely)
        Assert.That(hashA1, Is.Not.EqualTo(hashC));
    }

    [Test]
    public void Parse_WithInvalidFormat_ThrowsFormatException()
    {
        // Arrange
        var invalidInputs = new[]
        {
            "invalid-guid",
            "12345",
            "",
            "not-a-guid-at-all",
            "123e4567-e89b-12d3-a456", // incomplete
            "123e4567-e89b-12d3-a456-4266141740001234", // too long
            "ZZZZZZZZ-ZZZZ-ZZZZ-ZZZZ-ZZZZZZZZZZZZ" // invalid characters
        };

        // Act & Assert
        foreach (var invalidInput in invalidInputs)
        {
            Assert.Throws<FormatException>(() => Guid7.Parse(invalidInput),
                $"Expected FormatException for input: '{invalidInput}'");
        }
    }
}
