namespace PatientService.Infrastructure.Persistence.Configurations;

public class PatientDoctorAssignmentHistoryConfiguration : IEntityTypeConfiguration<PatientDoctorAssignmentHistory>
{
    public void Configure(EntityTypeBuilder<PatientDoctorAssignmentHistory> builder)
    {
        builder.ToTable("PatientDoctorAssignmentHistories");

        builder.HasKey(x => new { x.PatientId, x.SequenceNumber })
            .HasName("PK_PATIENT_DOCTOR_ASSIGNMENT_HISTORY");

        builder.Property(x => x.PatientId)
            .HasColumnName("PatientId")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(x => x.SequenceNumber)
            .HasColumnName("SequenceNumber")
            .HasColumnType("numeric(2,0)")
            .IsRequired();

        builder.Property(x => x.DoctorId)
            .HasColumnName("DoctorId")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(x => x.From)
            .HasColumnName("From")
            .HasColumnType("timestamp")
            .IsRequired();

        builder.Property(x => x.To)
            .HasColumnName("To")
            .HasColumnType("timestamp");

        builder.HasOne(x => x.Patient)
            .WithMany(p => p.DoctorAssignmentHistories)
            .HasForeignKey(x => x.PatientId)
            .HasConstraintName("FK_PATIENT_DOCTOR_ASSIGNMENT_HISTORY_PATIENT")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.PatientId)
            .HasDatabaseName("IX_PATIENT_DOCTOR_ASSIGNMENT_HISTORY_PATIENT");

        builder.HasIndex(x => x.DoctorId)
            .HasDatabaseName("IX_PATIENT_DOCTOR_ASSIGNMENT_HISTORY_DOCTOR");
    }
}
