using Microsoft.IdentityModel.JsonWebTokens;

namespace Boilerplatr.Security.JwtBearerToken;

public static partial class CustomClaims
{
    public const string TokenId = JwtRegisteredClaimNames.Jti;
    public const string SessionId = JwtRegisteredClaimNames.Sid;
    public const string UserId = JwtRegisteredClaimNames.Sub;
    public const string UserEmail = JwtRegisteredClaimNames.Email;
    public const string UserRole = "role";
}
