using DoctorService.Domain.Entities;

namespace DoctorService.Infrastructure.Persistence.Configurations;

public class LicenseAuthorityConfiguration : IEntityTypeConfiguration<LicenseAuthority>
{
    public void Configure(EntityTypeBuilder<LicenseAuthority> builder)
    {
        builder.ToTable("LicenseAuthorities");

        builder.HasKey(x => x.Code)
            .HasName("PK_LICENSE_AUTHORITY");

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
