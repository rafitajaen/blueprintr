using Blueprintr.Converters;
using Blueprintr.Shared;
using NUnit.Framework;

namespace Blueprintr.Tests.Converters;

[TestFixture]
public class Guid7ConverterTests
{
    [Test]
    public void ConvertToProvider_ShouldReturnGuidValue()
    {
        var converter = new Guid7Converter();
        var guid = Guid.NewGuid();
        var guid7 = Guid7.FromGuid(guid);

        var result = converter.ConvertToProvider(guid7);

        Assert.That(result, Is.EqualTo(guid));
    }

    [Test]
    public void ConvertFromProvider_ShouldReturnGuid7()
    {
        var converter = new Guid7Converter();
        var guid = Guid.NewGuid();

        var result = converter.ConvertFromProvider(guid);

        Assert.That(result, Is.InstanceOf<Guid7>());
        Assert.That(((Guid7)result!).Value, Is.EqualTo(guid));
    }
}
