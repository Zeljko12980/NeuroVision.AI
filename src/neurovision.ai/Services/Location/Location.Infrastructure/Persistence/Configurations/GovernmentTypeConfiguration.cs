using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LocationService.Domain.Entities;

namespace LocationService.Infrastructure.Persistence.Configurations;

public class GovernmentTypeConfiguration : IEntityTypeConfiguration<GovernmentType>
{
    public void Configure(EntityTypeBuilder<GovernmentType> builder)
    {
        builder.ToTable("GovernmentTypes");

        builder.HasKey(g => g.Code)
            .HasName("PK_GOVERNMENT_TYPE");

        builder.Property(g => g.Code)
            .HasColumnName("Code")
            .HasColumnType("varchar(10)")
            .IsRequired();

        builder.Property(g => g.Name)
            .HasColumnName("Name")
            .HasColumnType("varchar(120)")
            .IsRequired();

        builder.Property(g => g.Description)
            .HasColumnName("Description")
            .HasColumnType("varchar(256)");
    }
}