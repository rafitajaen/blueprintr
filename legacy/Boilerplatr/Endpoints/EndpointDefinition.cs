using Boilerplatr.Security.JwtBearerToken;
using Microsoft.AspNetCore.Authorization;

namespace Boilerplatr.Endpoints;

public abstract class EndpointDefinition : IEndpoint
{
    // public Type RequestType { get; private set; } = typeof(TRequest);
    // public Type ResponseType { get; private set; } = typeof(TResponse);
    public Type? ValidatorType { get; private set; }

    // Endpoint Description
    public string Verb { get; private set; } = string.Empty;
    public string Route { get; private set; } = string.Empty;
    public Delegate RequestHandler { get; private set; } = () => { };

    // Endpoint Security
    public bool AllowedAnonymous { get; private set; }
    public List<string> AllowedRoles { get; private set; } = [];
    public List<string> AllowedPolicies { get; private set; } = [];
    public List<string> AllowedClaimTypes { get; private set; } = [];
    public List<string> AllowedPermissions { get; private set; } = [];


    // Action Flags
    private bool _handlerIsSetted;
    private string SecurityPolicyName => $"Endpoint:Policy:{GetType().FullName}";

    public EndpointDefinition()
    {
        Configure();
        Validate();
    }

    public abstract void Configure();
    private void Validate()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(Verb);
        ArgumentException.ThrowIfNullOrWhiteSpace(Route);
        ArgumentNullException.ThrowIfNull(RequestHandler);

        if (!_handlerIsSetted)
        {
            throw new ArgumentException("Handler was not set");
        }

        if (AllowedAnonymous && AllowedAnonymous == RequiresAuthorization())
        {
            throw new ArgumentException("Endpoint security parameters include errors.");
        }

    }

    protected void AllowAnonymous() => AllowedAnonymous = true;
    public void AllowRoles(params string[] roles) => AllowedRoles.AddRange(roles);
    public void AllowClaims(params string[] claims) => AllowedClaimTypes.AddRange(claims);
    public void AllowPolicies(params string[] policies) => AllowedPolicies.AddRange(policies);
    public void AllowPermissions(params string[] permissions) => AllowedPermissions.AddRange(permissions);
    public void ValidateBy(Type type) => ValidatorType = type;


    public bool RequiresAuthentication() => !AllowedAnonymous;
    public bool RequiresAuthorization()
    {
        return AllowedPermissions.Count > 0
            || AllowedClaimTypes.Count > 0
            || AllowedRoles.Count > 0
            || AllowedPolicies.Count > 0;
        // || AuthSchemeNames?.Count > 0
        // || PolicyBuilder is not null
        // || PreBuiltUserPolicies is not null;
    }

    protected void Get(string route) => Define(route, EndpointVerbs.Get);
    protected void Post(string route) => Define(route, EndpointVerbs.Post);
    protected void Put(string route) => Define(route, EndpointVerbs.Put);
    protected void Delete(string route) => Define(route, EndpointVerbs.Delete);

    private void Define(string route, string verb)
    {
        Route = route;
        Verb = verb;
    }

    protected void Handle(Delegate handler)
    {
        RequestHandler = handler;
        _handlerIsSetted = true;
    }

    public string? GetSecurityPolicyName()
    {
        if (AllowedPermissions.Count > 0 || AllowedClaimTypes.Count > 0 || AllowedPolicies.Count > 1)
        {
            return SecurityPolicyName;
        }
        else if (AllowedPolicies.Count == 1)
        {
            return AllowedPolicies.ElementAt(0);
        }
        else
        {
            return null;
        }
    }

    public void AddSecurityPolicy(AuthorizationOptions options, JwtBearerTokenOptions jwtOptions)
    {
        if (AllowedPermissions.Count == 0 && AllowedClaimTypes.Count == 0 && AllowedPolicies.Count <= 1)
        {
            return;
        }

        options.AddPolicy
        (
            name: SecurityPolicyName,
            configurePolicy: builder =>
            {
                builder.RequireAuthenticatedUser();

                if (AllowedPermissions.Count > 0)
                {
                    builder.RequireAssertion
                    (
                        handler: context => AllowedPermissions.All
                        (
                            permission => context.User.Claims.Any
                            (
                                claim => string.Equals(claim.Type, jwtOptions.PermissionsClaimType, StringComparison.OrdinalIgnoreCase)
                                    && string.Equals(claim.Value, permission, StringComparison.Ordinal)
                            )
                        )
                    );
                }

                if (AllowedClaimTypes.Count > 0)
                {
                    builder.RequireAssertion
                    (
                        context => AllowedClaimTypes.All
                        (
                            claimType => context.User.Claims.Any(c => string.Equals(c.Type, claimType, StringComparison.OrdinalIgnoreCase))
                        )
                    );
                }

                if (AllowedPolicies.Count > 1)
                {
                    foreach (var policy in AllowedPolicies)
                    {
                        var authorizationPolicy = options.GetPolicy(policy);
                        ArgumentNullException.ThrowIfNull(authorizationPolicy);

                        builder.Combine(authorizationPolicy);
                    }
                }
            }
        );
    }
}
