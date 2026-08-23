namespace PatientService.Infrastructure.Persistence.Configurations;

public class PatientLanguageCoverageConfiguration : IEntityTypeConfiguration<PatientLanguageCoverage>
{
    public void Configure(EntityTypeBuilder<PatientLanguageCoverage> builder)
    {
        builder.ToTable("PatientLanguageCoverages");

        builder.HasKey(x => new { x.PatientId, x.LanguageCode })
            .HasName("PK_PATIENT_LANGUAGE_COVERAGE");

        builder.Property(x => x.PatientId)
            .HasColumnName("PatientId")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(x => x.LanguageCode)
            .HasColumnName("LanguageCode")
            .HasColumnType("varchar(10)")
            .IsRequired();

        builder.HasOne(x => x.Patient)
            .WithMany(p => p.LanguageCoverages)
            .HasForeignKey(x => x.PatientId)
            .HasConstraintName("FK_PATIENT_LANGUAGE_COVERAGE_PATIENT")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Language)
            .WithMany(l => l.Coverages)
            .HasForeignKey(x => x.LanguageCode)
            .HasConstraintName("FK_PATIENT_LANGUAGE_COVERAGE_LANGUAGE")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.LanguageCode)
            .HasDatabaseName("IX_PATIENT_LANGUAGE_COVERAGE_LANGUAGE");
    }
}
