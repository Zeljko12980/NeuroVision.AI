namespace NotificationService.Infrastructure.Persistence.Configurations;

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

public class NotificationTypeConfiguration : IEntityTypeConfiguration<NotificationType>
{
    public void Configure(EntityTypeBuilder<NotificationType> builder)
        => CatalogTable.Map(builder, "NotificationTypes", "PK_NOTIFICATION_TYPE");
}

public class NotificationSeverityConfiguration : IEntityTypeConfiguration<NotificationSeverity>
{
    public void Configure(EntityTypeBuilder<NotificationSeverity> builder)
        => CatalogTable.Map(builder, "NotificationSeverities", "PK_NOTIFICATION_SEVERITY");
}

public class NotificationChannelConfiguration : IEntityTypeConfiguration<NotificationChannel>
{
    public void Configure(EntityTypeBuilder<NotificationChannel> builder)
        => CatalogTable.Map(builder, "NotificationChannels", "PK_NOTIFICATION_CHANNEL");
}
