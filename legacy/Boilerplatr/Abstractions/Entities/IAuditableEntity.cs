using NodaTime;

namespace Boilerplatr.Abstractions.Entities;

public interface IAuditableEntity : IEntity
{
    Instant? DeletedAt { get; set; }
    Instant? CreatedAt { get; set; }
    Instant? UpdatedAt { get; set; }
    Instant? PublishedAt { get; set; }
    
    virtual bool IPublished(Instant time) => PublishedAt is not null && time >= PublishedAt;
}
