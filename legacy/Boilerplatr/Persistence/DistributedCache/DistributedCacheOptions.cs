namespace Boilerplatr.Persistence.DistributedCache;

public abstract class DistributedCacheOptions
{
    public abstract string IdentifierPrefix { get; set; }
    public abstract int SlidingExpirationMinutes { get; set; }
}