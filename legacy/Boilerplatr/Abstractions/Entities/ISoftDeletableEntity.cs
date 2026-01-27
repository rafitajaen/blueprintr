using NodaTime;

namespace Boilerplatr.Abstractions.Entities;

public interface ISoftDeletableEntity : IEntity
{
    Instant? DeletedAt { get; set; }
    virtual bool IsDeleted() => DeletedAt is not null;
}
