using DoctorService.Domain.Entities;

namespace DoctorService.Infrastructure.Persistence.Configurations;

public class DoctorStatusHistoryConfiguration : IEntityTypeConfiguration<DoctorStatusHistory>
{
    public void Configure(EntityTypeBuilder<DoctorStatusHistory> builder)
    {
        builder.ToTable("DoctorStatusHistories");

        builder.HasKey(x => new { x.DoctorId, x.SequenceNumber })
            .HasName("PK_DOCTOR_STATUS_HISTORY");

        builder.Property(x => x.DoctorId)
            .HasColumnName("DoctorId")
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

        builder.HasOne(x => x.Doctor)
            .WithMany(d => d.StatusHistories)
            .HasForeignKey(x => x.DoctorId)
            .HasConstraintName("FK_DOCTOR_STATUS_HISTORY_DOCTOR")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Status)
            .WithMany(s => s.Histories)
            .HasForeignKey(x => x.StatusCode)
            .HasConstraintName("FK_DOCTOR_STATUS_HISTORY_STATUS")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.DoctorId)
            .HasDatabaseName("IX_DOCTOR_STATUS_HISTORY_DOCTOR");

        builder.HasIndex(x => x.StatusCode)
            .HasDatabaseName("IX_DOCTOR_STATUS_HISTORY_STATUS");
    }
}
