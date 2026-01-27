using Boilerplatr.Shared;

namespace Boilerplatr.Abstractions.Entities;

public interface ISlugableEntity : IEntity
{
    Slug Slug { get; set; }
}
