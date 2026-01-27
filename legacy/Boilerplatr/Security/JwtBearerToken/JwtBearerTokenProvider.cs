using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;

using System.Text;
using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;
using NodaTime;

namespace Boilerplatr.Security.JwtBearerToken;

public interface IJwtBearerTokenProvider<TOptions, TClaims>
{
    TOptions Options { get; init; }
    
    CookieOptions GetCookieOptions(Instant now);
    ClaimsIdentity ExtractClaimsIdentity(string? token);
    Task<TokenValidationResult> ValidateAsync(string? token);
    bool TryGetClaims(string? token, [NotNullWhen(true)] out TClaims? claims);
    bool TryGenerateToken(TClaims claims, [NotNullWhen(true)] out string token);
    bool TryGetFromCookie(HttpContext context, [NotNullWhen(true)] out string? token);
    bool TryGetFromHeader(HttpContext context, [NotNullWhen(true)] out string? token);
    void DeleteCookieFrom(HttpContext context);
}

/// <summary>
/// Default JWT Token Handler that generates and reads JWT Tokens
/// </summary>
public abstract class JwtBearerTokenProvider<TOptions, TClaims> : IJwtBearerTokenProvider<TOptions, TClaims>
where TOptions : JwtBearerTokenOptions
where TClaims : JwtBearerTokenClaims
{
    public TOptions Options { get; init; }

    private readonly SecurityKey _securityKey;
    private readonly SigningCredentials _credentials;
    private readonly JsonWebTokenHandler _tokenHandler;
    private readonly TokenValidationParameters _tokenValidator;

    /// <summary>
    /// Public constructor of Token Manager
    /// </summary>
    public JwtBearerTokenProvider(TOptions options)
    {
        Options = options;
        _tokenHandler = new JsonWebTokenHandler();
        _securityKey = GenerateSecurityKey(options);
        _credentials = GenerateSigningCredentials(options);
        _tokenValidator = GenerateTokenValidationParameters(options, _securityKey);
    }

    public void DeleteCookieFrom(HttpContext context) => context.Response.Cookies.Delete(Options.CookieName);

    public bool TryGetFromCookie(HttpContext context, [NotNullWhen(true)] out string? token)
    {
        return context.TryGetCookie(Options.CookieName, out token);
    }

    public bool TryGetFromHeader(HttpContext context, [NotNullWhen(true)] out string? token)
    {
        token = context.Request.Headers.Authorization.ToString().Split(' ').ElementAtOrDefault(1);
        return !string.IsNullOrWhiteSpace(token);
    }

    /// <inheritdoc/>
    public abstract bool HasValidClaims(IEnumerable<Claim>? claims);

    /// <inheritdoc/>
    public bool TryGenerateToken(IEnumerable<Claim> claims, out string token, DateTime? utcNow = null)
    {
        token = string.Empty;

        if (!HasValidClaims(claims))
        {
            return false;
        }

        var now = utcNow ?? DateTime.UtcNow;
        var descriptor = new SecurityTokenDescriptor
        {
            IssuedAt = now,
            NotBefore = now,
            Expires = now.Add(TimeSpan.FromMinutes(Options.ExpirationMinutes)),

            Issuer = Options.Issuer,
            Audience = Options.Audience,

            SigningCredentials = _credentials,

            Subject = new ClaimsIdentity(claims),
        };

        token = _tokenHandler.CreateToken(descriptor);

        return !string.IsNullOrWhiteSpace(token);
    }

    public async Task<TokenValidationResult> ValidateAsync(string? token)
    {
        var tokenValidationResult = await ValidateTokenAsync(token);

        if (!HasValidClaims(tokenValidationResult.ClaimsIdentity?.Claims))
        {
            tokenValidationResult.IsValid = false;
        }

        return tokenValidationResult;
    }

    /// <inheritdoc/>
    private async Task<TokenValidationResult> ValidateTokenAsync(string? token)
    {
        try
        {
            return await _tokenHandler.ValidateTokenAsync(token, _tokenValidator);
        }
        catch (Exception)
        {
            return new TokenValidationResult();
        }
    }

    /// <inheritdoc/>
    public ClaimsIdentity ExtractClaimsIdentity(string? token)
    {
        try
        {
            return new ClaimsIdentity(_tokenHandler.ReadJsonWebToken(token).Claims);
        }
        catch (Exception)
        {
            return new ClaimsIdentity();
        }
    }

    public static TokenValidationParameters GenerateTokenValidationParameters(JwtBearerTokenOptions options, SecurityKey securityKey)
    {
        var key = Environment.GetEnvironmentVariable(options.SigningKey);

        return new TokenValidationParameters
        {
            IssuerSigningKey = securityKey,
            ValidateIssuerSigningKey = string.IsNullOrWhiteSpace(key),

            ValidateIssuer = options.Issuer is not null,
            ValidateAudience = options.Audience is not null,

            ClockSkew = TimeSpan.FromSeconds(options.ClockSkewSeconds)
        };
    }

    public static SigningCredentials GenerateSigningCredentials(JwtBearerTokenOptions options)
    {
        var key = Environment.GetEnvironmentVariable(options.SigningKey);

        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        switch (options.SigningStyle)
        {
            case JwtBearerTokenStyles.Symmetric:
                {
                    return new SigningCredentials
                    (
                        key: new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                        algorithm: options.SigningAlgorithm
                    );
                }
            case JwtBearerTokenStyles.Asymmetric:
                {
                    var rsa = RSA.Create();
                    if (options.KeyIsPemEncoded)
                    {
                        rsa.ImportFromPem(key);
                    }
                    else
                    {
                        rsa.ImportRSAPrivateKey(Convert.FromBase64String(key), out _);
                    }

                    return new SigningCredentials
                    (
                        key: new RsaSecurityKey(rsa),
                        algorithm: options.SigningAlgorithm
                    );
                }
            default:
                {
                    throw new InvalidOperationException("Jwt Bearer Token signing style is not specified");
                }
        }
    }

    public static SecurityKey GenerateSecurityKey(JwtBearerTokenOptions options)
    {
        var key = Environment.GetEnvironmentVariable(options.SigningKey);

        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        switch (options.SigningStyle)
        {
            case JwtBearerTokenStyles.Symmetric:
                {
                    return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
                }
            case JwtBearerTokenStyles.Asymmetric:
                {
                    var rsa = RSA.Create();

                    if (options.KeyIsPemEncoded)
                    {
                        rsa.ImportFromPem(key);
                    }
                    else
                    {
                        rsa.ImportRSAPublicKey(Convert.FromBase64String(key), out _);
                    }

                    return new RsaSecurityKey(rsa);
                }
            default:
                {
                    throw new InvalidOperationException("Jwt Bearer Token signing style is not specified");
                }
        }
    }

    public CookieOptions GetCookieOptions(Instant now)
    {
        return new CookieOptions()
        {
            HttpOnly = true,
            Expires = now.Plus(Duration.FromMinutes(Options.ExpirationMinutes)).ToDateTimeOffset(),
            SameSite = SameSiteMode.Strict,
            Secure = true,
        };
    }

    public abstract bool TryGenerateToken(TClaims claims, [NotNullWhen(true)] out string token);

    public abstract bool TryGetClaims(string? token, [NotNullWhen(true)] out TClaims? claims);
}
