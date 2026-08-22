using DoctorService.Domain.Entities;

namespace DoctorService.Infrastructure.Persistence.Configurations;

public class DoctorDegreeCoverageConfiguration : IEntityTypeConfiguration<DoctorDegreeCoverage>
{
    public void Configure(EntityTypeBuilder<DoctorDegreeCoverage> builder)
    {
        builder.ToTable("DoctorDegreeCoverages");

        builder.HasKey(x => new { x.DoctorId, x.DegreeTypeCode })
            .HasName("PK_DOCTOR_DEGREE_COVERAGE");

        builder.Property(x => x.DoctorId)
            .HasColumnName("DoctorId")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(x => x.DegreeTypeCode)
            .HasColumnName("DegreeTypeCode")
            .HasColumnType("varchar(10)")
            .IsRequired();

        builder.Property(x => x.InstitutionName)
            .HasColumnName("InstitutionName")
            .HasColumnType("varchar(150)");

        builder.Property(x => x.Year)
            .HasColumnName("Year")
            .HasColumnType("numeric(4,0)");

        builder.HasOne(x => x.Doctor)
            .WithMany(d => d.DegreeCoverages)
            .HasForeignKey(x => x.DoctorId)
            .HasConstraintName("FK_DOCTOR_DEGREE_COVERAGE_DOCTOR")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.DegreeType)
            .WithMany(t => t.Coverages)
            .HasForeignKey(x => x.DegreeTypeCode)
            .HasConstraintName("FK_DOCTOR_DEGREE_COVERAGE_DEGREE_TYPE")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.DegreeTypeCode)
            .HasDatabaseName("IX_DOCTOR_DEGREE_COVERAGE_DEGREE_TYPE");
    }
}
