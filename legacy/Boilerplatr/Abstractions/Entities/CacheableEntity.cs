namespace Boilerplatr.Abstractions.Entities;

public abstract class CacheableEntity<TName, T> : Entity<T>
{
    public string GetCacheId() => GetCacheId(Id);
    public static string GetCacheId(T Id) => $"{nameof(TName).ToLowerInvariant()}:{Id}";
}
