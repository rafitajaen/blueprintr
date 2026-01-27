using Boilerplatr.Shared;
using NodaTime;

namespace Boilerplatr.Identities;

public interface IIdentity
{

    /// <summary>
    /// Gets or sets the email address for this user.
    /// </summary>
    string Email { get; set; }

    /// <summary>
    /// Gets or sets the normalized email address for this user.
    /// </summary>
    NormalizedString NormalizedEmail { get; set; }

    /// <summary>
    /// Gets or sets the user name for this user.
    /// </summary>
    string Username { get; set; }

    /// <summary>
    /// Gets or sets the normalized user name for this user.
    /// </summary>
    NormalizedString NormalizedUsername { get; set; }

    /// <summary>
    /// Gets or sets a salted and hashed representation of the password for this user.
    /// </summary>
    byte[]? PasswordSalt { get; set; }

    /// <summary>
    /// Gets or sets a salted and hashed representation of the password for this user.
    /// </summary>
    byte[]? PasswordHash { get; set; }

    string RoleId { get; set; }

    /// <summary>
    /// Gets or sets a flag indicating if a user has confirmed their email address.
    /// </summary>
    Instant? EmailConfirmedAt { get; set; }

    bool IsEmailConfirmed() => EmailConfirmedAt is not null;
}
