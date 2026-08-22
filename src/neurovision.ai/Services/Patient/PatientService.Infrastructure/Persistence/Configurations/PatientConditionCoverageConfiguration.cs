namespace PatientService.Infrastructure.Persistence.Configurations;

public class PatientConditionCoverageConfiguration : IEntityTypeConfiguration<PatientConditionCoverage>
{
    public void Configure(EntityTypeBuilder<PatientConditionCoverage> builder)
    {
        builder.ToTable("PatientConditionCoverages");

        builder.HasKey(x => new { x.PatientId, x.ConditionCode })
            .HasName("PK_PATIENT_CONDITION_COVERAGE");

        builder.Property(x => x.PatientId)
            .HasColumnName("PatientId")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(x => x.ConditionCode)
            .HasColumnName("ConditionCode")
            .HasColumnType("varchar(10)")
            .IsRequired();

        builder.Property(x => x.DiagnosedYear)
            .HasColumnName("DiagnosedYear")
            .HasColumnType("numeric(4,0)");

        builder.Property(x => x.Note)
            .HasColumnName("Note")
            .HasColumnType("varchar(500)");

        builder.HasOne(x => x.Patient)
            .WithMany(p => p.ConditionCoverages)
            .HasForeignKey(x => x.PatientId)
            .HasConstraintName("FK_PATIENT_CONDITION_COVERAGE_PATIENT")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Condition)
            .WithMany(c => c.Coverages)
            .HasForeignKey(x => x.ConditionCode)
            .HasConstraintName("FK_PATIENT_CONDITION_COVERAGE_CONDITION")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ConditionCode)
            .HasDatabaseName("IX_PATIENT_CONDITION_COVERAGE_CONDITION");
    }
}
