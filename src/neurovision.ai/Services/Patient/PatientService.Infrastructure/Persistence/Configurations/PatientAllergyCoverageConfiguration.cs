namespace PatientService.Infrastructure.Persistence.Configurations;

public class PatientAllergyCoverageConfiguration : IEntityTypeConfiguration<PatientAllergyCoverage>
{
    public void Configure(EntityTypeBuilder<PatientAllergyCoverage> builder)
    {
        builder.ToTable("PatientAllergyCoverages");

        builder.HasKey(x => new { x.PatientId, x.AllergyCode })
            .HasName("PK_PATIENT_ALLERGY_COVERAGE");

        builder.Property(x => x.PatientId)
            .HasColumnName("PatientId")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(x => x.AllergyCode)
            .HasColumnName("AllergyCode")
            .HasColumnType("varchar(10)")
            .IsRequired();

        builder.Property(x => x.Note)
            .HasColumnName("Note")
            .HasColumnType("varchar(500)");

        builder.HasOne(x => x.Patient)
            .WithMany(p => p.AllergyCoverages)
            .HasForeignKey(x => x.PatientId)
            .HasConstraintName("FK_PATIENT_ALLERGY_COVERAGE_PATIENT")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Allergy)
            .WithMany(a => a.Coverages)
            .HasForeignKey(x => x.AllergyCode)
            .HasConstraintName("FK_PATIENT_ALLERGY_COVERAGE_ALLERGY")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.AllergyCode)
            .HasDatabaseName("IX_PATIENT_ALLERGY_COVERAGE_ALLERGY");
    }
}
