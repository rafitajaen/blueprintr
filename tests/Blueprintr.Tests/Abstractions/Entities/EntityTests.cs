using Blueprintr.Abstractions.Entities;
using NUnit.Framework;

namespace Blueprintr.Tests.Abstractions.Entities;

/// <summary>
/// Tests for <see cref="Entity{T}"/> abstract base class.
/// </summary>
[TestFixture]
public class EntityTests
{
    /// <summary>
    /// Concrete implementation of Entity for testing with Guid identifier.
    /// </summary>
    private class GuidEntity : Entity<Guid>
    {
        public override Guid Id { get; init; }
        public string? Name { get; set; }
    }

    /// <summary>
    /// Concrete implementation of Entity for testing with int identifier.
    /// </summary>
    private class IntEntity : Entity<int>
    {
        public override int Id { get; init; }
        public string? Description { get; set; }
    }

    /// <summary>
    /// Concrete implementation of Entity for testing with string identifier.
    /// </summary>
    private class StringEntity : Entity<string>
    {
        public override string Id { get; init; } = string.Empty;
        public decimal Value { get; set; }
    }

    [Test]
    public void Entity_CanCreateConcreteImplementation()
    {
        // Arrange
        var guidId = Guid.NewGuid();
        var intId = 42;
        var stringId = "entity-123";

        // Act
        var guidEntity = new GuidEntity { Id = guidId, Name = "Test Entity" };
        var intEntity = new IntEntity { Id = intId, Description = "Integer Entity" };
        var stringEntity = new StringEntity { Id = stringId, Value = 99.99m };

        // Assert
        Assert.That(guidEntity, Is.Not.Null);
        Assert.That(guidEntity.Id, Is.EqualTo(guidId));
        Assert.That(guidEntity.Name, Is.EqualTo("Test Entity"));

        Assert.That(intEntity, Is.Not.Null);
        Assert.That(intEntity.Id, Is.EqualTo(intId));
        Assert.That(intEntity.Description, Is.EqualTo("Integer Entity"));

        Assert.That(stringEntity, Is.Not.Null);
        Assert.That(stringEntity.Id, Is.EqualTo(stringId));
        Assert.That(stringEntity.Value, Is.EqualTo(99.99m));
    }

    [Test]
    public void Entity_IdProperty_IsSettable()
    {
        // Arrange
        var initialId = Guid.NewGuid();

        // Act - Create entity with Id set via init accessor
        var entity = new GuidEntity { Id = initialId };

        // Assert
        Assert.That(entity.Id, Is.EqualTo(initialId));
        Assert.That(entity.Id, Is.Not.EqualTo(Guid.Empty));

        // Verify that different entities can have different IDs
        var secondId = Guid.NewGuid();
        var secondEntity = new GuidEntity { Id = secondId };
        Assert.That(secondEntity.Id, Is.EqualTo(secondId));
        Assert.That(entity.Id, Is.Not.EqualTo(secondEntity.Id));
    }

    [Test]
    public void Entity_InheritsFromIEntity()
    {
        // Arrange
        var entity = new GuidEntity { Id = Guid.NewGuid() };
        var intEntity = new IntEntity { Id = 100 };
        var stringEntity = new StringEntity { Id = "test-id" };

        // Act & Assert - Verify all concrete implementations are IEntity
        Assert.That(entity, Is.InstanceOf<IEntity>());
        Assert.That(intEntity, Is.InstanceOf<IEntity>());
        Assert.That(stringEntity, Is.InstanceOf<IEntity>());

        // Verify they can be assigned to IEntity reference
        IEntity entityAsInterface = entity;
        IEntity intEntityAsInterface = intEntity;
        IEntity stringEntityAsInterface = stringEntity;

        Assert.That(entityAsInterface, Is.Not.Null);
        Assert.That(intEntityAsInterface, Is.Not.Null);
        Assert.That(stringEntityAsInterface, Is.Not.Null);

        // Verify Entity<T> itself inherits from IEntity
        Assert.That(typeof(Entity<Guid>).GetInterfaces(), Does.Contain(typeof(IEntity)));
        Assert.That(typeof(Entity<int>).GetInterfaces(), Does.Contain(typeof(IEntity)));
        Assert.That(typeof(Entity<string>).GetInterfaces(), Does.Contain(typeof(IEntity)));
    }
}
