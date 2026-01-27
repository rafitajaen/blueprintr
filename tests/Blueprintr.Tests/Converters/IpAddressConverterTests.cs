using System.Net;
using Blueprintr.Converters;
using NUnit.Framework;

namespace Blueprintr.Tests.Converters;

[TestFixture]
public class IpAddressConverterTests
{
    [Test]
    public void ConvertToProvider_ShouldReturnStringRepresentation()
    {
        var converter = new IpAddressConverter();
        var ipAddress = IPAddress.Parse("192.168.1.1");

        var result = converter.ConvertToProvider(ipAddress);

        Assert.That(result, Is.EqualTo("192.168.1.1"));
    }

    [Test]
    public void ConvertFromProvider_ShouldReturnIPAddress()
    {
        var converter = new IpAddressConverter();
        var ipString = "192.168.1.1";

        var result = converter.ConvertFromProvider(ipString);

        Assert.That(result, Is.InstanceOf<IPAddress>());
        Assert.That(result!.ToString(), Is.EqualTo("192.168.1.1"));
    }

    [Test]
    public void ConvertToProvider_IPv6_ShouldReturnStringRepresentation()
    {
        var converter = new IpAddressConverter();
        var ipAddress = IPAddress.Parse("2001:0db8:85a3:0000:0000:8a2e:0370:7334");

        var result = converter.ConvertToProvider(ipAddress);

        Assert.That(result, Is.EqualTo("2001:db8:85a3::8a2e:370:7334"));
    }

    [Test]
    public void ConvertFromProvider_IPv6_ShouldReturnIPAddress()
    {
        var converter = new IpAddressConverter();
        var ipString = "2001:db8:85a3::8a2e:370:7334";

        var result = converter.ConvertFromProvider(ipString);

        Assert.That(result, Is.InstanceOf<IPAddress>());
        Assert.That(result!.ToString(), Is.EqualTo("2001:db8:85a3::8a2e:370:7334"));
    }
}
