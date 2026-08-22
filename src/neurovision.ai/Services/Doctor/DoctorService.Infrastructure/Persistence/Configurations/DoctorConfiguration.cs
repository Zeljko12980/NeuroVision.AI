using DoctorService.Domain.Entities;

namespace DoctorService.Infrastructure.Persistence.Configurations;

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.ToTable("Doctors");

        builder.HasKey(x => x.Id)
            .HasName("PK_DOCTOR");

        builder.Property(x => x.Id)
            .HasColumnName("Id")
            .HasColumnType("uuid")
            .ValueGeneratedNever();

        builder.Property(x => x.FirstName)
            .HasColumnName("FirstName")
            .HasColumnType("varchar(100)")
            .IsRequired();

        builder.Property(x => x.LastName)
            .HasColumnName("LastName")
            .HasColumnType("varchar(100)")
            .IsRequired();

        builder.Property(x => x.Email)
            .HasColumnName("Email")
            .HasColumnType("varchar(150)")
            .IsRequired();

        builder.Property(x => x.Phone)
            .HasColumnName("Phone")
            .HasColumnType("varchar(50)")
            .IsRequired();

        builder.Property(x => x.LicenseNumber)
            .HasColumnName("LicenseNumber")
            .HasColumnType("varchar(50)")
            .IsRequired();

        builder.Property(x => x.LicenseAuthorityCode)
            .HasColumnName("LicenseAuthorityCode")
            .HasColumnType("varchar(10)");

        builder.Property(x => x.CurrentSpecializationCode)
            .HasColumnName("CurrentSpecializationCode")
            .HasColumnType("varchar(10)")
            .IsRequired();

        builder.Property(x => x.CurrentStatusCode)
            .HasColumnName("CurrentStatusCode")
            .HasColumnType("varchar(10)")
            .IsRequired();

        builder.Property(x => x.ProfilePictureUrl)
            .HasColumnName("ProfilePictureUrl")
            .HasColumnType("varchar(500)");

        builder.Property(x => x.Bio)
            .HasColumnName("Bio")
            .HasColumnType("varchar(2000)");

        builder.Property(x => x.CurrentHealthInstitutionId)
            .HasColumnName("CurrentHealthInstitutionId")
            .HasColumnType("int");

        builder.Property(x => x.CurrentInstitutionName)
            .HasColumnName("CurrentInstitutionName")
            .HasColumnType("varchar(150)");

        builder.Property(x => x.IsAvailable)
            .HasColumnName("IsAvailable")
            .HasColumnType("boolean")
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.LastActive)
            .HasColumnName("LastActive")
            .HasColumnType("timestamp")
            .IsRequired();

        builder.Property(x => x.AverageRating)
            .HasColumnName("AverageRating")
            .HasColumnType("numeric(3,2)")
            .IsRequired()
            .HasDefaultValue(0m);

        builder.Property(x => x.TotalReviews)
            .HasColumnName("TotalReviews")
            .HasColumnType("int")
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("timestamp")
            .IsRequired();

        builder.HasOne(x => x.Status)
            .WithMany(s => s.Doctors)
            .HasForeignKey(x => x.CurrentStatusCode)
            .HasConstraintName("FK_DOCTOR_STATUS")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CurrentSpecialization)
            .WithMany(s => s.Doctors)
            .HasForeignKey(x => x.CurrentSpecializationCode)
            .HasConstraintName("FK_DOCTOR_SPECIALIZATION")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.LicenseAuthority)
            .WithMany(a => a.Doctors)
            .HasForeignKey(x => x.LicenseAuthorityCode)
            .HasConstraintName("FK_DOCTOR_LICENSE_AUTHORITY")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.Email)
            .IsUnique()
            .HasDatabaseName("UX_DOCTOR_EMAIL");

        builder.HasIndex(x => x.LicenseNumber)
            .IsUnique()
            .HasDatabaseName("UX_DOCTOR_LICENSE");

        builder.HasIndex(x => x.CurrentStatusCode)
            .HasDatabaseName("IX_DOCTOR_STATUS");

        builder.HasIndex(x => x.CurrentSpecializationCode)
            .HasDatabaseName("IX_DOCTOR_SPECIALIZATION");

        builder.HasIndex(x => x.LicenseAuthorityCode)
            .HasDatabaseName("IX_DOCTOR_LICENSE_AUTHORITY");

        builder.HasIndex(x => x.LastName)
            .HasDatabaseName("IX_DOCTOR_LASTNAME");

        builder.HasIndex(x => new { x.IsAvailable, x.CurrentStatusCode })
            .HasDatabaseName("IX_DOCTOR_AVAILABLE_STATUS");
    }
}
