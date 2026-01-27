using Blueprintr.Converters;
using NUnit.Framework;

namespace Blueprintr.Tests.Converters;

[TestFixture]
public class UlidConverterTests
{
    [Test]
    public void ConvertToProvider_ShouldReturnGuidValue()
    {
        var converter = new UlidConverter();
        var guid = Guid.NewGuid();
        var ulid = new Ulid(guid);

        var result = converter.ConvertToProvider(ulid);

        Assert.That(result, Is.EqualTo(guid));
    }

    [Test]
    public void ConvertFromProvider_ShouldReturnUlid()
    {
        var converter = new UlidConverter();
        var guid = Guid.NewGuid();

        var result = converter.ConvertFromProvider(guid);

        Assert.That(result, Is.InstanceOf<Ulid>());
        Assert.That(((Ulid)result!).ToGuid(), Is.EqualTo(guid));
    }
}
