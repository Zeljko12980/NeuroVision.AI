using LocationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LocationService.Infrastructure.Persistence.Configurations;

public class HealthInstitutionTypeConfiguration : IEntityTypeConfiguration<HealthInstitutionType>
{
    public void Configure(EntityTypeBuilder<HealthInstitutionType> builder)
    {
        builder.ToTable("HealthInstitutionTypes");

        builder.HasKey(x => x.Code)
            .HasName("PK_HealthInstitutionTypes");

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
            .HasColumnType("varchar(256)");
    }
}