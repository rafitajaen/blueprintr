using System.Net;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Blueprintr.Converters;

/// <summary>
/// Converts <see cref="IPAddress"/> to <see cref="string"/> for database storage.
/// </summary>
/// <remarks>
/// This converter allows Entity Framework Core to automatically convert between
/// the <see cref="IPAddress"/> type and string representation for database storage.
/// Supports both IPv4 and IPv6 addresses.
/// Added in version 1.0.0.
/// </remarks>
public class IpAddressConverter : ValueConverter<IPAddress, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IpAddressConverter"/> class.
    /// </summary>
    /// <remarks>Added in version 1.0.0.</remarks>
    public IpAddressConverter() : base
    (
        ip => ip.ToString(),
        s => IPAddress.Parse(s)
    ) { }
}
