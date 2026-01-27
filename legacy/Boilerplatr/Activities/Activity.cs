using Boilerplatr.Abstractions.Entities;
using Boilerplatr.Shared;
using NodaTime;

namespace Boilerplatr.Activities;

public sealed class Activity : Entity<Guid7>
{
    public override required Guid7 Id { get; init; }
    public required string Event { get; set; }
    public required string Data { get; set; }
    public required string IpAddress { get; set; }
    public required Instant At { get; set; }

   
}
