namespace Blueprintr.Abstractions.Entities;

/// <summary>
/// Abstract base class for entities with a strongly-typed identifier.
/// </summary>
/// <typeparam name="T">The type of the entity's identifier.</typeparam>
/// <remarks>
/// This class provides a base implementation for entities in the domain model.
/// Entities are objects that have a distinct identity that runs through time and different states.
/// The identity is represented by the <see cref="Id"/> property.
/// Added in version 1.0.0.
/// </remarks>
public abstract class Entity<T> : IEntity
{
    /// <summary>
    /// Gets the unique identifier for this entity.
    /// </summary>
    /// <remarks>
    /// The identifier uniquely distinguishes this entity from all other entities of the same type.
    /// Added in version 1.0.0.
    /// </remarks>
    public abstract T Id { get; init; }
}
