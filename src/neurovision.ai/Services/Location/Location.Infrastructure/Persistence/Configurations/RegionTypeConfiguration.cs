using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LocationService.Domain.Entities;

namespace LocationService.Infrastructure.Persistence.Configurations;

public class RegionTypeConfiguration : IEntityTypeConfiguration<RegionType>
{
    public void Configure(EntityTypeBuilder<RegionType> builder)
    {
        builder.ToTable("RegionTypes");

        builder.HasKey(x => x.Code)
            .HasName("PK_REGION_TYPE");

        builder.Property(x => x.Code)
            .HasColumnName("Code")
            .HasColumnType("varchar(10)")
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("Name")
            .HasColumnType("varchar(120)")
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("Description")
            .HasColumnType("varchar(265)");
    }
}