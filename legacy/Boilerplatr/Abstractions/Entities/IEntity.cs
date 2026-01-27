using Boilerplatr.Abstractions.DomainEvents;

namespace Boilerplatr.Abstractions.Entities;

public interface IEntity
{
    IReadOnlyList<IDomainEvent> GetEvents();
    void ClearEvents();
    void RaiseEvent(IDomainEvent domainEvent);
}
