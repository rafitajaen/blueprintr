using Boilerplatr.Security.AccessToken;
using Boilerplatr.Security.RefreshToken;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Boilerplatr.Security.Authentication;

public static class AuthenticationDependencyInjection
{
    public static WebApplicationBuilder AddCustomAuthentication(this WebApplicationBuilder builder)
    {
        /* Access Token */
        var accessTokenOptions = builder.Configuration.GetRequiredSection("AccessToken").Get<AccessTokenOptions>();

        ArgumentNullException.ThrowIfNull(accessTokenOptions);
        accessTokenOptions.TryValidateOptions();

        /* Refresh Token */
        var refreshTokenOptions = builder.Configuration.GetRequiredSection("RefreshToken").Get<RefreshTokenOptions>();

        ArgumentNullException.ThrowIfNull(refreshTokenOptions);
        refreshTokenOptions.TryValidateOptions();

        /* Token Options */
        builder.Services.Configure<AccessTokenOptions>(builder.Configuration.GetRequiredSection("AccessToken"));
        builder.Services.Configure<RefreshTokenOptions>(builder.Configuration.GetRequiredSection("RefreshToken"));

        /* Token Services */
        builder.Services.AddSingleton<IAccessTokenProvider, AccessTokenProvider>();
        builder.Services.AddSingleton<IRefreshTokenProvider, RefreshTokenProvider>();

        builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();

        return builder;
    }

    public static IApplicationBuilder UseCustomAuthentication<TIdentity, T>(this IApplicationBuilder app)
    {
        return app.UseMiddleware<AuthenticationMiddleware<TIdentity, T>>();
    }

    // public static WebApplicationBuilder AddAuthenticationJwtBearer(this WebApplicationBuilder builder)
    // {

    //     var options = builder.Configuration.GetRequiredSection("AccessToken").Get<AccessTokenOptions>();

    //     ArgumentNullException.ThrowIfNull(options);
    //     options.TryValidateOptions();

    //     builder.Services
    //         .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    //         .AddJwtBearer
    //         (
    //             builder =>
    //             {
    //                 var securityKey = JwtBearerTokenHandler.GenerateSecurityKey(options);

    //                 builder.TokenValidationParameters = new TokenValidationParameters()
    //                 {
    //                     IssuerSigningKey = securityKey,
    //                     ValidateIssuerSigningKey = securityKey is not null,
    //                     ValidateLifetime = true,
    //                     ClockSkew = TimeSpan.FromSeconds(options.ClockSkewSeconds),

    //                     ValidIssuer = options.Issuer,
    //                     ValidateIssuer = options.Issuer is not null,
    //                     ValidAudience = options.Audience,
    //                     ValidateAudience = options.Audience is not null,

    //                     NameClaimType = options.NameClaimType,
    //                     RoleClaimType = options.RoleClaimType,
    //                 };

    //                 builder.MapInboundClaims = false;
    //             }
    //         );

    //     return builder;

    // }

}
