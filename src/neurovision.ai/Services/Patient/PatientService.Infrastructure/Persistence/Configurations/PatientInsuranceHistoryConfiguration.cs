namespace PatientService.Infrastructure.Persistence.Configurations;

public class PatientInsuranceHistoryConfiguration : IEntityTypeConfiguration<PatientInsuranceHistory>
{
    public void Configure(EntityTypeBuilder<PatientInsuranceHistory> builder)
    {
        builder.ToTable("PatientInsuranceHistories");

        builder.HasKey(x => new { x.PatientId, x.SequenceNumber })
            .HasName("PK_PATIENT_INSURANCE_HISTORY");

        builder.Property(x => x.PatientId)
            .HasColumnName("PatientId")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(x => x.SequenceNumber)
            .HasColumnName("SequenceNumber")
            .HasColumnType("numeric(2,0)")
            .IsRequired();

        builder.Property(x => x.PayerCode)
            .HasColumnName("PayerCode")
            .HasColumnType("varchar(10)")
            .IsRequired();

        builder.Property(x => x.PolicyNumber)
            .HasColumnName("PolicyNumber")
            .HasColumnType("varchar(50)")
            .IsRequired();

        builder.Property(x => x.From)
            .HasColumnName("From")
            .HasColumnType("timestamp")
            .IsRequired();

        builder.Property(x => x.To)
            .HasColumnName("To")
            .HasColumnType("timestamp");

        builder.HasOne(x => x.Patient)
            .WithMany(p => p.InsuranceHistories)
            .HasForeignKey(x => x.PatientId)
            .HasConstraintName("FK_PATIENT_INSURANCE_HISTORY_PATIENT")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Payer)
            .WithMany(p => p.Histories)
            .HasForeignKey(x => x.PayerCode)
            .HasConstraintName("FK_PATIENT_INSURANCE_HISTORY_PAYER")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.PatientId)
            .HasDatabaseName("IX_PATIENT_INSURANCE_HISTORY_PATIENT");

        builder.HasIndex(x => x.PayerCode)
            .HasDatabaseName("IX_PATIENT_INSURANCE_HISTORY_PAYER");
    }
}
