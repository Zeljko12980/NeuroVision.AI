namespace PatientService.Infrastructure.Persistence.Configurations;

public class PatientEmergencyContactConfiguration : IEntityTypeConfiguration<PatientEmergencyContact>
{
    public void Configure(EntityTypeBuilder<PatientEmergencyContact> builder)
    {
        builder.ToTable("PatientEmergencyContacts");

        builder.HasKey(x => new { x.PatientId, x.SequenceNumber })
            .HasName("PK_PATIENT_EMERGENCY_CONTACT");

        builder.Property(x => x.PatientId)
            .HasColumnName("PatientId")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(x => x.SequenceNumber)
            .HasColumnName("SequenceNumber")
            .HasColumnType("numeric(2,0)")
            .IsRequired();

        builder.Property(x => x.FullName)
            .HasColumnName("FullName")
            .HasColumnType("varchar(150)")
            .IsRequired();

        builder.Property(x => x.Phone)
            .HasColumnName("Phone")
            .HasColumnType("varchar(50)")
            .IsRequired();

        builder.Property(x => x.RelationshipCode)
            .HasColumnName("RelationshipCode")
            .HasColumnType("varchar(10)")
            .IsRequired();

        builder.HasOne(x => x.Patient)
            .WithMany(p => p.EmergencyContacts)
            .HasForeignKey(x => x.PatientId)
            .HasConstraintName("FK_PATIENT_EMERGENCY_CONTACT_PATIENT")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Relationship)
            .WithMany(r => r.Contacts)
            .HasForeignKey(x => x.RelationshipCode)
            .HasConstraintName("FK_PATIENT_EMERGENCY_CONTACT_RELATIONSHIP")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.PatientId)
            .HasDatabaseName("IX_PATIENT_EMERGENCY_CONTACT_PATIENT");

        builder.HasIndex(x => x.RelationshipCode)
            .HasDatabaseName("IX_PATIENT_EMERGENCY_CONTACT_RELATIONSHIP");
    }
}
