using DoctorService.Domain.Entities;

namespace DoctorService.Infrastructure.Persistence.Configurations;

public class DoctorStatusConfiguration : IEntityTypeConfiguration<DoctorStatus>
{
    public void Configure(EntityTypeBuilder<DoctorStatus> builder)
    {
        builder.ToTable("DoctorStatuses");

        builder.HasKey(x => x.Code)
            .HasName("PK_DOCTOR_STATUS");

        builder.Property(x => x.Code)
            .HasColumnName("Code")
            .HasColumnType("varchar(10)")
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("Name")
            .HasColumnType("varchar(120)")
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("Description")
            .HasColumnType("varchar(256)");
    }
}
