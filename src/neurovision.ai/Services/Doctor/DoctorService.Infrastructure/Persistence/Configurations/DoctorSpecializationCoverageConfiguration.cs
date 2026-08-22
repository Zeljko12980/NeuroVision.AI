using DoctorService.Domain.Entities;

namespace DoctorService.Infrastructure.Persistence.Configurations;

public class DoctorSpecializationCoverageConfiguration : IEntityTypeConfiguration<DoctorSpecializationCoverage>
{
    public void Configure(EntityTypeBuilder<DoctorSpecializationCoverage> builder)
    {
        builder.ToTable("DoctorSpecializationCoverages");

        builder.HasKey(x => new { x.DoctorId, x.SpecializationCode })
            .HasName("PK_DOCTOR_SPECIALIZATION_COVERAGE");

        builder.Property(x => x.DoctorId)
            .HasColumnName("DoctorId")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(x => x.SpecializationCode)
            .HasColumnName("SpecializationCode")
            .HasColumnType("varchar(10)")
            .IsRequired();

        builder.Property(x => x.IsPrimary)
            .HasColumnName("IsPrimary")
            .HasColumnType("boolean")
            .IsRequired();

        builder.Property(x => x.From)
            .HasColumnName("From")
            .HasColumnType("timestamp")
            .IsRequired();

        builder.Property(x => x.To)
            .HasColumnName("To")
            .HasColumnType("timestamp");

        builder.HasOne(x => x.Doctor)
            .WithMany(d => d.SpecializationCoverages)
            .HasForeignKey(x => x.DoctorId)
            .HasConstraintName("FK_DOCTOR_SPECIALIZATION_COVERAGE_DOCTOR")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Specialization)
            .WithMany(s => s.Coverages)
            .HasForeignKey(x => x.SpecializationCode)
            .HasConstraintName("FK_DOCTOR_SPECIALIZATION_COVERAGE_SPECIALIZATION")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.SpecializationCode)
            .HasDatabaseName("IX_DOCTOR_SPECIALIZATION_COVERAGE_SPECIALIZATION");
    }
}
