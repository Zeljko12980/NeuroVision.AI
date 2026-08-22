using DoctorService.Domain.Entities;

namespace DoctorService.Infrastructure.Persistence.Configurations;

public class DegreeTypeConfiguration : IEntityTypeConfiguration<DegreeType>
{
    public void Configure(EntityTypeBuilder<DegreeType> builder)
    {
        builder.ToTable("DegreeTypes");

        builder.HasKey(x => x.Code)
            .HasName("PK_DEGREE_TYPE");

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
