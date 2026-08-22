namespace PatientService.Infrastructure.Persistence.Configurations;

internal static class CatalogTable
{
    public static void Map<T>(EntityTypeBuilder<T> builder, string table, string pkName)
        where T : class
    {
        builder.ToTable(table);

        builder.HasKey("Code")
            .HasName(pkName);

        builder.Property("Code")
            .HasColumnName("Code")
            .HasColumnType("varchar(10)")
            .IsRequired();

        builder.Property("Name")
            .HasColumnName("Name")
            .HasColumnType("varchar(120)")
            .IsRequired();

        builder.Property("Description")
            .HasColumnName("Description")
            .HasColumnType("varchar(256)");
    }
}

public class PatientStatusConfiguration : IEntityTypeConfiguration<PatientStatus>
{
    public void Configure(EntityTypeBuilder<PatientStatus> builder)
        => CatalogTable.Map(builder, "PatientStatuses", "PK_PATIENT_STATUS");
}

public class GenderConfiguration : IEntityTypeConfiguration<Gender>
{
    public void Configure(EntityTypeBuilder<Gender> builder)
        => CatalogTable.Map(builder, "Genders", "PK_GENDER");
}

public class BloodTypeConfiguration : IEntityTypeConfiguration<BloodType>
{
    public void Configure(EntityTypeBuilder<BloodType> builder)
        => CatalogTable.Map(builder, "BloodTypes", "PK_BLOOD_TYPE");
}

public class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
        => CatalogTable.Map(builder, "Languages", "PK_LANGUAGE");
}

public class AllergyConfiguration : IEntityTypeConfiguration<Allergy>
{
    public void Configure(EntityTypeBuilder<Allergy> builder)
        => CatalogTable.Map(builder, "Allergies", "PK_ALLERGY");
}

public class ConditionConfiguration : IEntityTypeConfiguration<Condition>
{
    public void Configure(EntityTypeBuilder<Condition> builder)
        => CatalogTable.Map(builder, "Conditions", "PK_CONDITION");
}

public class InsurancePayerConfiguration : IEntityTypeConfiguration<InsurancePayer>
{
    public void Configure(EntityTypeBuilder<InsurancePayer> builder)
        => CatalogTable.Map(builder, "InsurancePayers", "PK_INSURANCE_PAYER");
}

public class RelationshipTypeConfiguration : IEntityTypeConfiguration<RelationshipType>
{
    public void Configure(EntityTypeBuilder<RelationshipType> builder)
        => CatalogTable.Map(builder, "RelationshipTypes", "PK_RELATIONSHIP_TYPE");
}

public class ConsentTypeConfiguration : IEntityTypeConfiguration<ConsentType>
{
    public void Configure(EntityTypeBuilder<ConsentType> builder)
        => CatalogTable.Map(builder, "ConsentTypes", "PK_CONSENT_TYPE");
}
