using Boilerplatr.Abstractions.DomainEvents;

namespace Boilerplatr.Abstractions.Entities;

public abstract class Entity<T> : IEntity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public abstract T Id { get; init; }

    public IReadOnlyList<IDomainEvent> GetEvents() => _domainEvents;

    public void ClearEvents() => _domainEvents.Clear();

    public void RaiseEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
}
