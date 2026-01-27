using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;

namespace Boilerplatr.Security;

public interface IIdentityRepository<TIdentity, T>
{
    bool TryGetUserId(HttpContext context, [NotNullWhen(true)] out T userId);
    Task<TIdentity?> GetAsync(T Id, CancellationToken cancellationToken = default);
}
