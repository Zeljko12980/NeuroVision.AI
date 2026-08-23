namespace PatientService.Infrastructure.Persistence.Configurations;

public class PatientConsentCoverageConfiguration : IEntityTypeConfiguration<PatientConsentCoverage>
{
    public void Configure(EntityTypeBuilder<PatientConsentCoverage> builder)
    {
        builder.ToTable("PatientConsentCoverages");

        builder.HasKey(x => new { x.PatientId, x.ConsentTypeCode })
            .HasName("PK_PATIENT_CONSENT_COVERAGE");

        builder.Property(x => x.PatientId)
            .HasColumnName("PatientId")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(x => x.ConsentTypeCode)
            .HasColumnName("ConsentTypeCode")
            .HasColumnType("varchar(10)")
            .IsRequired();

        builder.Property(x => x.From)
            .HasColumnName("From")
            .HasColumnType("timestamp")
            .IsRequired();

        builder.Property(x => x.To)
            .HasColumnName("To")
            .HasColumnType("timestamp");

        builder.HasOne(x => x.Patient)
            .WithMany(p => p.ConsentCoverages)
            .HasForeignKey(x => x.PatientId)
            .HasConstraintName("FK_PATIENT_CONSENT_COVERAGE_PATIENT")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ConsentType)
            .WithMany(c => c.Coverages)
            .HasForeignKey(x => x.ConsentTypeCode)
            .HasConstraintName("FK_PATIENT_CONSENT_COVERAGE_TYPE")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ConsentTypeCode)
            .HasDatabaseName("IX_PATIENT_CONSENT_COVERAGE_TYPE");
    }
}
