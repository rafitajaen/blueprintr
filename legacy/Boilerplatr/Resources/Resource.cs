using Boilerplatr.Abstractions.Entities;
using Boilerplatr.Shared;
using NodaTime;

namespace Boilerplatr.Resources;

public sealed class Resource : Entity<Guid7>, ISoftDeletableEntity, ISlugableEntity
{
    public override Guid7 Id { get; init; }

    public required string MimeType { get; set; }
    public required byte[] Content { get; set; }
    public required Slug Slug { get; set; }

    public Instant? DeletedAt { get; set; }
}
