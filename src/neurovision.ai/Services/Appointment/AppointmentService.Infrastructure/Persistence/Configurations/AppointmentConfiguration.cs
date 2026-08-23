namespace AppointmentService.Infrastructure.Persistence.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointments");

        builder.HasKey(x => x.Id)
            .HasName("PK_APPOINTMENT");

        builder.Property(x => x.Id)
            .HasColumnName("Id")
            .HasColumnType("uuid")
            .ValueGeneratedNever();

        builder.Property(x => x.PatientId)
            .HasColumnName("PatientId")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(x => x.DoctorId)
            .HasColumnName("DoctorId")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(x => x.TypeCode)
            .HasColumnName("TypeCode")
            .HasColumnType("varchar(10)")
            .IsRequired();

        builder.Property(x => x.StatusCode)
            .HasColumnName("StatusCode")
            .HasColumnType("varchar(10)")
            .IsRequired();

        builder.Property(x => x.StartsAt)
            .HasColumnName("StartsAt")
            .HasColumnType("timestamp")
            .IsRequired();

        builder.Property(x => x.EndsAt)
            .HasColumnName("EndsAt")
            .HasColumnType("timestamp")
            .IsRequired();

        builder.Property(x => x.Title)
            .HasColumnName("Title")
            .HasColumnType("varchar(120)")
            .IsRequired();

        builder.Property(x => x.Notes)
            .HasColumnName("Notes")
            .HasColumnType("varchar(512)");

        builder.Property(x => x.HealthInstitutionId)
            .HasColumnName("HealthInstitutionId")
            .HasColumnType("int");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("timestamp")
            .IsRequired();

        builder.Property(x => x.CancelledAt)
            .HasColumnName("CancelledAt")
            .HasColumnType("timestamp");

        builder.Property(x => x.CompletedAt)
            .HasColumnName("CompletedAt")
            .HasColumnType("timestamp");

        builder.HasOne(x => x.Type)
            .WithMany(t => t.Appointments)
            .HasForeignKey(x => x.TypeCode)
            .HasConstraintName("FK_APPOINTMENT_TYPE")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Status)
            .WithMany(s => s.Appointments)
            .HasForeignKey(x => x.StatusCode)
            .HasConstraintName("FK_APPOINTMENT_STATUS")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.TypeCode)
            .HasDatabaseName("IX_APPOINTMENT_TYPE");

        builder.HasIndex(x => x.StatusCode)
            .HasDatabaseName("IX_APPOINTMENT_STATUS");

        builder.HasIndex(x => new { x.DoctorId, x.StartsAt })
            .HasDatabaseName("IX_APPOINTMENT_DOCTOR_START");

        builder.HasIndex(x => new { x.PatientId, x.StartsAt })
            .HasDatabaseName("IX_APPOINTMENT_PATIENT_START");
    }
}
