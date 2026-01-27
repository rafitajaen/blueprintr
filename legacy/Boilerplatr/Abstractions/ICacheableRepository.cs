using Boilerplatr.Abstractions.Entities;

namespace Boilerplatr.Abstractions;

public interface ICacheableRepository<TEntity, T> where TEntity : CacheableEntity<TEntity, T>
{
    Task<TEntity?> GetAsync(T Id, CancellationToken cancellationToken = default);
}
