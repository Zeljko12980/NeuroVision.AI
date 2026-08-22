namespace PatientService.Infrastructure.Persistence.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("Patients");

        builder.HasKey(x => x.Id)
            .HasName("PK_PATIENT");

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

        builder.Property(x => x.DateOfBirth)
            .HasColumnName("DateOfBirth")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(x => x.GenderCode)
            .HasColumnName("GenderCode")
            .HasColumnType("varchar(10)")
            .IsRequired();

        builder.Property(x => x.BloodTypeCode)
            .HasColumnName("BloodTypeCode")
            .HasColumnType("varchar(10)");

        builder.Property(x => x.NationalId)
            .HasColumnName("NationalId")
            .HasColumnType("varchar(20)");

        builder.Property(x => x.CurrentStatusCode)
            .HasColumnName("CurrentStatusCode")
            .HasColumnType("varchar(10)")
            .IsRequired();

        builder.Property(x => x.ProfilePictureUrl)
            .HasColumnName("ProfilePictureUrl")
            .HasColumnType("varchar(500)");

        builder.Property(x => x.Notes)
            .HasColumnName("Notes")
            .HasColumnType("varchar(2000)");

        builder.Property(x => x.CurrentHealthInstitutionId)
            .HasColumnName("CurrentHealthInstitutionId")
            .HasColumnType("int");

        builder.Property(x => x.CurrentInstitutionName)
            .HasColumnName("CurrentInstitutionName")
            .HasColumnType("varchar(150)");

        builder.Property(x => x.AssignedDoctorId)
            .HasColumnName("AssignedDoctorId")
            .HasColumnType("uuid");

        builder.Property(x => x.CurrentInsurancePayerCode)
            .HasColumnName("CurrentInsurancePayerCode")
            .HasColumnType("varchar(10)");

        builder.Property(x => x.CurrentInsurancePolicyNumber)
            .HasColumnName("CurrentInsurancePolicyNumber")
            .HasColumnType("varchar(50)");

        builder.Property(x => x.AddressLine)
            .HasColumnName("AddressLine")
            .HasColumnType("varchar(250)");

        builder.Property(x => x.SettlementId)
            .HasColumnName("SettlementId")
            .HasColumnType("int");

        builder.Property(x => x.MunicipalityId)
            .HasColumnName("MunicipalityId")
            .HasColumnType("int");

        builder.Property(x => x.CountryId)
            .HasColumnName("CountryId")
            .HasColumnType("int");

        builder.Property(x => x.HeightCm)
            .HasColumnName("HeightCm")
            .HasColumnType("numeric(5,1)");

        builder.Property(x => x.WeightKg)
            .HasColumnName("WeightKg")
            .HasColumnType("numeric(5,1)");

        builder.Property(x => x.LastActive)
            .HasColumnName("LastActive")
            .HasColumnType("timestamp")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("timestamp")
            .IsRequired();

        builder.HasOne(x => x.Status)
            .WithMany(s => s.Patients)
            .HasForeignKey(x => x.CurrentStatusCode)
            .HasConstraintName("FK_PATIENT_STATUS")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Gender)
            .WithMany(g => g.Patients)
            .HasForeignKey(x => x.GenderCode)
            .HasConstraintName("FK_PATIENT_GENDER")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.BloodType)
            .WithMany(b => b.Patients)
            .HasForeignKey(x => x.BloodTypeCode)
            .HasConstraintName("FK_PATIENT_BLOOD_TYPE")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CurrentInsurancePayer)
            .WithMany()
            .HasForeignKey(x => x.CurrentInsurancePayerCode)
            .HasConstraintName("FK_PATIENT_INSURANCE_PAYER")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.Email)
            .IsUnique()
            .HasDatabaseName("UX_PATIENT_EMAIL");

        builder.HasIndex(x => x.NationalId)
            .IsUnique()
            .HasFilter("\"NationalId\" IS NOT NULL")
            .HasDatabaseName("UX_PATIENT_NATIONAL_ID");

        builder.HasIndex(x => x.CurrentStatusCode)
            .HasDatabaseName("IX_PATIENT_STATUS");

        builder.HasIndex(x => x.GenderCode)
            .HasDatabaseName("IX_PATIENT_GENDER");

        builder.HasIndex(x => x.BloodTypeCode)
            .HasDatabaseName("IX_PATIENT_BLOOD_TYPE");

        builder.HasIndex(x => x.AssignedDoctorId)
            .HasDatabaseName("IX_PATIENT_ASSIGNED_DOCTOR");

        builder.HasIndex(x => x.LastName)
            .HasDatabaseName("IX_PATIENT_LASTNAME");
    }
}
