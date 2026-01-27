using Boilerplatr.Persistence.EntityFramework.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Boilerplatr.Emails;

public class EmailMapping : IEntityTypeConfiguration<Email>
{
    public void Configure(EntityTypeBuilder<Email> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .HasConversion(new Guid7Converter());

        builder.Property(i => i.TenantId)
            .HasConversion(new Guid7Converter());

        builder.OwnsOne(e => e.Message);

        // Relations
        builder
            .HasOne(e => e.Tenant)
            .WithMany()
            .HasForeignKey(e => e.TenantId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
