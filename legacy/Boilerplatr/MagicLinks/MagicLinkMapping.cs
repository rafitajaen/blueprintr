using Boilerplatr.Persistence.EntityFramework.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Boilerplatr.MagicLinks;

public class MagicLinkMapping : IEntityTypeConfiguration<MagicLink>
{
    public void Configure(EntityTypeBuilder<MagicLink> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .HasConversion(new UlidConverter());

        builder.Property(i => i.UserId)
            .HasConversion(new Guid7Converter());

        builder.Property(i => i.TenantId)
            .HasConversion(new Guid7Converter());

        builder.Property(i => i.EntityId)
            .HasConversion(new Guid7Converter());

        builder.Property(i => i.Email)
            .HasConversion(new NormalizedStringConverter());
    }
}
