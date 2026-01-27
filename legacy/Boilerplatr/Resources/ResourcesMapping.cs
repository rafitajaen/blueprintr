using Boilerplatr.Persistence.EntityFramework.Converters;
using Boilerplatr.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Boilerplatr.Resources;

public class ResourcesMapping : IEntityTypeConfiguration<Resource>
{
    public void Configure(EntityTypeBuilder<Resource> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .HasConversion(new Guid7Converter());

        builder.Property(i => i.Slug)
            .HasConversion(new SlugConverter());

        builder.HasIndex(s => s.Slug).IsUnique();

    }
}
