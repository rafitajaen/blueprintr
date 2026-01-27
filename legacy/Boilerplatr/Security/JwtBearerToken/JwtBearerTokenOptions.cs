using Microsoft.IdentityModel.Tokens;

namespace Boilerplatr.Security.JwtBearerToken;

/// <summary>
/// Options for interacting with authentication jwt tokens
/// </summary>
public abstract class JwtBearerTokenOptions
{
    public string CookieName { get; set; } = default!;

    /// <summary>
    /// The key used to sign jwts symmetrically or the base64 encoded private-key when jwts are signed asymmetrically.
    /// </summary>
    public string SigningKey { get; set; } = default!;

    /// <summary>
    /// Specifies how tokens are to be signed. Symmetric or Asymmetric.
    /// </summary>
    public JwtBearerTokenStyles SigningStyle { get; set; } = JwtBearerTokenStyles.Symmetric;

    /// <summary>
    /// Security algorithm used to sign keys. Defaults to HmacSha256 for symmetric keys.
    /// </summary>
    public string SigningAlgorithm { get; set; } = SecurityAlgorithms.HmacSha256Signature;

    /// <summary>
    /// Specifies whether the key is pem encoded.
    /// </summary>
    public bool KeyIsPemEncoded { get; set; }

    /// <summary>
    /// Specifies the jwt token validity time.
    /// </summary>
    public int ExpirationMinutes { get; set; } = 60;

    /// <summary>
    /// Specifies the ClockSkewSeconds time.
    /// </summary>
    public int ClockSkewSeconds { get; set; }

    /// <summary>
    /// The value for the 'audience' claim.
    /// </summary>
    public string? Audience { get; set; }

    /// <summary>
    /// The value for the 'issuer' claim.
    /// </summary>
    public string? Issuer { get; set; }

    /// <summary>
    /// The compression algorithm for compressing the token payload.
    /// </summary>
    public string? CompressionAlgorithm { get; set; }

    /// <summary>
    /// Specify a custom claim type used to identity the name of a user principal. Defaults to `name`.
    /// </summary>
    public string NameClaimType { get; set; } = "name";

    /// <summary>
    /// Specify a custom claim type used to identify permissions of a user principal. defaults to `permission`.
    /// </summary>
    public string RoleClaimType { get; set; } = "role";

    /// <summary>
    /// Specify a custom claim type used to identify roles of a user principal. defaults to `role`.
    /// </summary>
    public string PermissionsClaimType { get; set; } = "permissions";

    public virtual bool TryValidateOptions()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(SigningKey);

        if (SigningStyle is JwtBearerTokenStyles.Asymmetric && SigningAlgorithm is SecurityAlgorithms.HmacSha256Signature)
        {
            throw new InvalidOperationException($"Invalid '{nameof(SigningAlgorithm)}' when creating an asymmetric signing Json Web Token.");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(NameClaimType);
        ArgumentException.ThrowIfNullOrWhiteSpace(RoleClaimType);
        ArgumentException.ThrowIfNullOrWhiteSpace(PermissionsClaimType);

        return true;
    }
}
