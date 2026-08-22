using DoctorService.Domain.Entities;

namespace DoctorService.Infrastructure.Persistence.Configurations;

public class DoctorAffiliationHistoryConfiguration : IEntityTypeConfiguration<DoctorAffiliationHistory>
{
    public void Configure(EntityTypeBuilder<DoctorAffiliationHistory> builder)
    {
        builder.ToTable("DoctorAffiliationHistories");

        builder.HasKey(x => new { x.DoctorId, x.SequenceNumber })
            .HasName("PK_DOCTOR_AFFILIATION_HISTORY");

        builder.Property(x => x.DoctorId)
            .HasColumnName("DoctorId")
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

        builder.HasOne(x => x.Doctor)
            .WithMany(d => d.AffiliationHistories)
            .HasForeignKey(x => x.DoctorId)
            .HasConstraintName("FK_DOCTOR_AFFILIATION_HISTORY_DOCTOR")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.DoctorId)
            .HasDatabaseName("IX_DOCTOR_AFFILIATION_HISTORY_DOCTOR");
    }
}
