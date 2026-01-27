namespace Boilerplatr.Identities;

public interface IIdentityFeature<TIdentity>
{
    TIdentity Identity { get; init; }
}

public sealed record IdentityFeature<TIdentity>(TIdentity Identity) : IIdentityFeature<TIdentity>;
