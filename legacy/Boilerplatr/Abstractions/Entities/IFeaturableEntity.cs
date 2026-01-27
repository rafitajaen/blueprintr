namespace Boilerplatr.Abstractions.Entities;

public interface IFeaturableEntity : IEntity
{
    string? FeaturedImageUrl { get; set; }
}
