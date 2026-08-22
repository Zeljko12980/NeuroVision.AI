using DoctorService.Domain.Entities;

namespace DoctorService.Infrastructure.Persistence.Configurations;

public class WorkingSlotConfiguration : IEntityTypeConfiguration<WorkingSlot>
{
    public void Configure(EntityTypeBuilder<WorkingSlot> builder)
    {
        builder.ToTable("WorkingSlots");

        builder.HasKey(x => new { x.DoctorId, x.DayOfWeek, x.SequenceNumber })
            .HasName("PK_WORKING_SLOT");

        builder.Property(x => x.DoctorId)
            .HasColumnName("DoctorId")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(x => x.DayOfWeek)
            .HasColumnName("DayOfWeek")
            .HasColumnType("smallint")
            .IsRequired();

        builder.Property(x => x.SequenceNumber)
            .HasColumnName("SequenceNumber")
            .HasColumnType("numeric(2,0)")
            .IsRequired();

        builder.Property(x => x.Start)
            .HasColumnName("Start")
            .HasColumnType("time")
            .IsRequired();

        builder.Property(x => x.End)
            .HasColumnName("End")
            .HasColumnType("time")
            .IsRequired();

        builder.Property(x => x.ValidFrom)
            .HasColumnName("ValidFrom")
            .HasColumnType("timestamp")
            .IsRequired();

        builder.Property(x => x.ValidTo)
            .HasColumnName("ValidTo")
            .HasColumnType("timestamp");

        builder.HasOne(x => x.Doctor)
            .WithMany(d => d.WorkingSlots)
            .HasForeignKey(x => x.DoctorId)
            .HasConstraintName("FK_WORKING_SLOT_DOCTOR")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.DoctorId)
            .HasDatabaseName("IX_WORKING_SLOT_DOCTOR");
    }
}
