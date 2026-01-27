using Boilerplatr.Persistence.EntityFramework.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Boilerplatr.Activities;

public class ActivityMapping : IEntityTypeConfiguration<Activity>
{
    public void Configure(EntityTypeBuilder<Activity> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .HasConversion(new Guid7Converter());

        builder.Property(i => i.Data)
            .HasColumnType("json");

        // builder.Property(d => d.IpAddress)
        //     .HasConversion(new IpAddressConverter())
        //     .HasColumnType("inet");  
    }
}
