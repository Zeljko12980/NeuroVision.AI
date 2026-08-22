using DoctorService.Domain.Entities;

namespace DoctorService.Infrastructure.Persistence.Configurations;

public class DoctorLicenseHistoryConfiguration : IEntityTypeConfiguration<DoctorLicenseHistory>
{
    public void Configure(EntityTypeBuilder<DoctorLicenseHistory> builder)
    {
        builder.ToTable("DoctorLicenseHistories");

        builder.HasKey(x => new { x.DoctorId, x.SequenceNumber })
            .HasName("PK_DOCTOR_LICENSE_HISTORY");

        builder.Property(x => x.DoctorId)
            .HasColumnName("DoctorId")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(x => x.SequenceNumber)
            .HasColumnName("SequenceNumber")
            .HasColumnType("numeric(2,0)")
            .IsRequired();

        builder.Property(x => x.LicenseNumber)
            .HasColumnName("LicenseNumber")
            .HasColumnType("varchar(50)")
            .IsRequired();

        builder.Property(x => x.LicenseAuthorityCode)
            .HasColumnName("LicenseAuthorityCode")
            .HasColumnType("varchar(10)");

        builder.Property(x => x.From)
            .HasColumnName("From")
            .HasColumnType("timestamp")
            .IsRequired();

        builder.Property(x => x.To)
            .HasColumnName("To")
            .HasColumnType("timestamp");

        builder.HasOne(x => x.Doctor)
            .WithMany(d => d.LicenseHistories)
            .HasForeignKey(x => x.DoctorId)
            .HasConstraintName("FK_DOCTOR_LICENSE_HISTORY_DOCTOR")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.LicenseAuthority)
            .WithMany(a => a.Histories)
            .HasForeignKey(x => x.LicenseAuthorityCode)
            .HasConstraintName("FK_DOCTOR_LICENSE_HISTORY_AUTHORITY")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.DoctorId)
            .HasDatabaseName("IX_DOCTOR_LICENSE_HISTORY_DOCTOR");

        builder.HasIndex(x => x.LicenseAuthorityCode)
            .HasDatabaseName("IX_DOCTOR_LICENSE_HISTORY_AUTHORITY");
    }
}
