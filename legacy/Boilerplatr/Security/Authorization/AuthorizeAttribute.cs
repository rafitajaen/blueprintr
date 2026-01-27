using Boilerplatr.Identities;

namespace Boilerplatr.Security.Authorization;

public interface IAuthorizeAttribute
{
    string[] Roles { get; set; }
    string[] Permissions { get; set; }
    string[] Claims { get; set; }
}

/// <summary>
/// Specifies the class this attribute is applied to requires authorization.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class AuthorizeAttribute : Attribute, IAuthorizeAttribute
{
    // * Role-based authorization
    public string[] Roles { get; set; } = [];

    // * Permission-based authorization
    public string[] Permissions { get; set; } = [];

    // * Claims-based authorization
    public string[] Claims { get; set; } = [];
}
