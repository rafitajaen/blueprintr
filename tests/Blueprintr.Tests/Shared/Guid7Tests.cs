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
}
