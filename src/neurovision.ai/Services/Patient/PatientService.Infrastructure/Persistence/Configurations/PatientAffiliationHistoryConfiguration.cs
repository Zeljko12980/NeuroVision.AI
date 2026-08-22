namespace PatientService.Infrastructure.Persistence.Configurations;

public class PatientAffiliationHistoryConfiguration : IEntityTypeConfiguration<PatientAffiliationHistory>
{
    public void Configure(EntityTypeBuilder<PatientAffiliationHistory> builder)
    {
        builder.ToTable("PatientAffiliationHistories");

        builder.HasKey(x => new { x.PatientId, x.SequenceNumber })
            .HasName("PK_PATIENT_AFFILIATION_HISTORY");

        builder.Property(x => x.PatientId)
            .HasColumnName("PatientId")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(x => x.SequenceNumber)
            .HasColumnName("SequenceNumber")
            .HasColumnType("numeric(2,0)")
            .IsRequired();

        builder.Property(x => x.HealthInstitutionId)
            .HasColumnName("HealthInstitutionId")
            .HasColumnType("int");

        builder.Property(x => x.InstitutionName)
            .HasColumnName("InstitutionName")
            .HasColumnType("varchar(150)")
            .IsRequired();

        builder.Property(x => x.From)
            .HasColumnName("From")
            .HasColumnType("timestamp")
            .IsRequired();

        builder.Property(x => x.To)
            .HasColumnName("To")
            .HasColumnType("timestamp");

        builder.HasOne(x => x.Patient)
            .WithMany(p => p.AffiliationHistories)
            .HasForeignKey(x => x.PatientId)
            .HasConstraintName("FK_PATIENT_AFFILIATION_HISTORY_PATIENT")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.PatientId)
            .HasDatabaseName("IX_PATIENT_AFFILIATION_HISTORY_PATIENT");
    }
}
