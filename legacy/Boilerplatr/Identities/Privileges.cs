using System.Security.Claims;

namespace Boilerplatr.Identities;

/// <summary>
/// Collection of user privileges to be included in the jwt or cookie.
/// </summary>
public sealed class Privileges
{
    /// <summary>
    /// Claims of the user
    /// </summary>
    public List<Claim> Claims { get; } = [];

    /// <summary>
    /// Roles of the user
    /// </summary>
    public List<string> Roles { get; } = [];

    /// <summary>
    /// Allowed permissions of the user
    /// </summary>
    public List<string> Permissions { get; } = [];

    public IEnumerable<Claim> ExtractClaims(string roleClaimType, string permissionsClaimType)
    {
        return
        [
            .. Claims,
            .. Roles.Select(role => new Claim(roleClaimType, role)),
            .. Permissions.Select(permission => new Claim(permissionsClaimType, permission))
        ];
    }

}
