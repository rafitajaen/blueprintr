using Boilerplatr.Abstractions.Entities;

namespace Boilerplatr.Identities;

public partial class Role : Entity<int>
{
    public override required int Id { get; init; }

    public required string Name { get; set; }

    public IEnumerable<Permission> Permissions { get; set; } = [];

    public static Role Guest => new()
    {
        Id = (int) BaseRoles.Guest,
        Name = nameof(BaseRoles.Guest)
    };

    public static Role Suscriber => new()
    {
        Id = (int)BaseRoles.Suscriber,
        Name = nameof(BaseRoles.Suscriber)
    };

    public static Role Lead => new()
    {
        Id = (int)BaseRoles.Lead,
        Name = nameof(BaseRoles.Lead)
    };

    public static Role Administrator => new()
    {
        Id = (int)BaseRoles.Administrator,
        Name = nameof(BaseRoles.Administrator)
    };
    
    public static Role SuperAdmin => new()
    {
        Id = (int)BaseRoles.SuperAdmin,
        Name = nameof(BaseRoles.SuperAdmin)
    };

    public bool HasPermission(Permission permission) => Id == (int) BaseRoles.SuperAdmin || Permissions.Any(x => x.Id == permission.Id);
}
