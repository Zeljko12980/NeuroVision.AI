using DoctorService.Domain.Entities;

namespace DoctorService.Infrastructure.Persistence.Configurations;

public class DoctorLanguageCoverageConfiguration : IEntityTypeConfiguration<DoctorLanguageCoverage>
{
    public void Configure(EntityTypeBuilder<DoctorLanguageCoverage> builder)
    {
        builder.ToTable("DoctorLanguageCoverages");

        builder.HasKey(x => new { x.DoctorId, x.LanguageCode })
            .HasName("PK_DOCTOR_LANGUAGE_COVERAGE");

        builder.Property(x => x.DoctorId)
            .HasColumnName("DoctorId")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(x => x.LanguageCode)
            .HasColumnName("LanguageCode")
            .HasColumnType("varchar(10)")
            .IsRequired();

        builder.HasOne(x => x.Doctor)
            .WithMany(d => d.LanguageCoverages)
            .HasForeignKey(x => x.DoctorId)
            .HasConstraintName("FK_DOCTOR_LANGUAGE_COVERAGE_DOCTOR")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Language)
            .WithMany(l => l.Coverages)
            .HasForeignKey(x => x.LanguageCode)
            .HasConstraintName("FK_DOCTOR_LANGUAGE_COVERAGE_LANGUAGE")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.LanguageCode)
            .HasDatabaseName("IX_DOCTOR_LANGUAGE_COVERAGE_LANGUAGE");
    }
}
