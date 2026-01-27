using Boilerplatr.Persistence.EntityFramework.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Boilerplatr.Tenants;

public class TenantMapping : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        // Key
        builder.HasKey(t => t.Id);
        
        // Index
        builder.HasIndex(t => t.Host).IsUnique();

        // Conversions
        builder.Property(t => t.Id)
            .HasConversion(new Guid7Converter());

        // Owned types
        builder.OwnsOne(t => t.Email);
        builder.OwnsOne(t => t.Social);
        builder.OwnsOne(t => t.Analytics);
    }
}
