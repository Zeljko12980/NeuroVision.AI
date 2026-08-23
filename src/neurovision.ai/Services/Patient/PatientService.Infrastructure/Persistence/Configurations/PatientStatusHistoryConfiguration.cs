namespace PatientService.Infrastructure.Persistence.Configurations;

public class PatientStatusHistoryConfiguration : IEntityTypeConfiguration<PatientStatusHistory>
{
    public void Configure(EntityTypeBuilder<PatientStatusHistory> builder)
    {
        builder.ToTable("PatientStatusHistories");

        builder.HasKey(x => new { x.PatientId, x.SequenceNumber })
            .HasName("PK_PATIENT_STATUS_HISTORY");

        builder.Property(x => x.PatientId)
            .HasColumnName("PatientId")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(x => x.SequenceNumber)
            .HasColumnName("SequenceNumber")
            .HasColumnType("numeric(2,0)")
            .IsRequired();

        builder.Property(x => x.StatusCode)
            .HasColumnName("StatusCode")
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
            .WithMany(p => p.StatusHistories)
            .HasForeignKey(x => x.PatientId)
            .HasConstraintName("FK_PATIENT_STATUS_HISTORY_PATIENT")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Status)
            .WithMany(s => s.Histories)
            .HasForeignKey(x => x.StatusCode)
            .HasConstraintName("FK_PATIENT_STATUS_HISTORY_STATUS")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.PatientId)
            .HasDatabaseName("IX_PATIENT_STATUS_HISTORY_PATIENT");

        builder.HasIndex(x => x.StatusCode)
            .HasDatabaseName("IX_PATIENT_STATUS_HISTORY_STATUS");
    }
}
