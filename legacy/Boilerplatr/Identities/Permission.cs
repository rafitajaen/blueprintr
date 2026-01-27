using Boilerplatr.Abstractions.Entities;

namespace Boilerplatr.Identities;

public partial class Permission : Entity<int>
{
    public override required int Id { get; init; }
    public required string Name { get; set; }
}
