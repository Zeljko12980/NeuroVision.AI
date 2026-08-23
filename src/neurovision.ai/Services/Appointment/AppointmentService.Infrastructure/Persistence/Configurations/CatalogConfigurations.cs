namespace AppointmentService.Infrastructure.Persistence.Configurations;

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

public class AppointmentTypeConfiguration : IEntityTypeConfiguration<AppointmentType>
{
    public void Configure(EntityTypeBuilder<AppointmentType> builder)
        => CatalogTable.Map(builder, "AppointmentTypes", "PK_APPOINTMENT_TYPE");
}

public class AppointmentStatusConfiguration : IEntityTypeConfiguration<AppointmentStatus>
{
    public void Configure(EntityTypeBuilder<AppointmentStatus> builder)
        => CatalogTable.Map(builder, "AppointmentStatuses", "PK_APPOINTMENT_STATUS");
}
