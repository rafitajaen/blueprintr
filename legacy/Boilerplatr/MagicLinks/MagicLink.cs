using Boilerplatr.Abstractions.Entities;
using Boilerplatr.Shared;
using NodaTime;

namespace Boilerplatr.MagicLinks;

public class MagicLink : Entity<Ulid>
{
    public required override Ulid Id { get; init; }
    public required Guid7 UserId { get; set; }
    public Guid7? TenantId { get; set; }
    public Guid7? EntityId { get; set; }
    public required NormalizedString Email { get; set; }
    public string? ReturnUrl { get; set; }
    public required Instant CreatedAt { get; set; }
    public required Instant ExpiredAt { get; set; }
    public bool HasBeenUsed { get; set; }

    public bool IsValid(Instant now)
    {
        return !HasBeenUsed && ExpiredAt > now;
    }
}
