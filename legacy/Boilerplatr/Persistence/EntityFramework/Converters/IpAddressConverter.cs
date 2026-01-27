using System.Net;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Boilerplatr.Persistence.EntityFramework.Converters;

public class IpAddressConverter : ValueConverter<IPAddress, string>
{
    public IpAddressConverter() : base
    (
        ip => ip.ToString(),
        s => IPAddress.Parse(s)
    ) { }
}
