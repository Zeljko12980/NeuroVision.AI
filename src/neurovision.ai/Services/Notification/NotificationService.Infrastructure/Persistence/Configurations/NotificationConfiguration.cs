namespace NotificationService.Infrastructure.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");

        builder.HasKey(x => x.Id)
            .HasName("PK_NOTIFICATION");

        builder.Property(x => x.Id)
            .HasColumnName("Id")
            .HasColumnType("uuid")
            .ValueGeneratedNever();

        builder.Property(x => x.RecipientUserId)
            .HasColumnName("RecipientUserId")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(x => x.TypeCode)
            .HasColumnName("TypeCode")
            .HasColumnType("varchar(10)")
            .IsRequired();

        builder.Property(x => x.SeverityCode)
            .HasColumnName("SeverityCode")
            .HasColumnType("varchar(10)")
            .IsRequired();

        builder.Property(x => x.Title)
            .HasColumnName("Title")
            .HasColumnType("varchar(120)")
            .IsRequired();

        builder.Property(x => x.Message)
            .HasColumnName("Message")
            .HasColumnType("varchar(512)")
            .IsRequired();

        builder.Property(x => x.Payload)
            .HasColumnName("Payload")
            .HasColumnType("jsonb");

        builder.Property(x => x.RelatedEntityType)
            .HasColumnName("RelatedEntityType")
            .HasColumnType("varchar(50)");

        builder.Property(x => x.RelatedEntityId)
            .HasColumnName("RelatedEntityId")
            .HasColumnType("uuid");

        builder.Property(x => x.HealthInstitutionId)
            .HasColumnName("HealthInstitutionId")
            .HasColumnType("int");

        builder.Property(x => x.SourceEventId)
            .HasColumnName("SourceEventId")
            .HasColumnType("uuid");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("timestamp")
            .IsRequired();

        builder.Property(x => x.ReadAt)
            .HasColumnName("ReadAt")
            .HasColumnType("timestamp");

        builder.HasOne(x => x.Type)
            .WithMany(t => t.Notifications)
            .HasForeignKey(x => x.TypeCode)
            .HasConstraintName("FK_NOTIFICATION_TYPE")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Severity)
            .WithMany(s => s.Notifications)
            .HasForeignKey(x => x.SeverityCode)
            .HasConstraintName("FK_NOTIFICATION_SEVERITY")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.TypeCode)
            .HasDatabaseName("IX_NOTIFICATION_TYPE");

        builder.HasIndex(x => x.SeverityCode)
            .HasDatabaseName("IX_NOTIFICATION_SEVERITY");

        builder.HasIndex(x => new { x.RecipientUserId, x.CreatedAt })
            .IsDescending(false, true)
            .HasDatabaseName("IX_NOTIFICATION_RECIPIENT_CREATED");

        builder.HasIndex(x => x.RecipientUserId)
            .HasFilter("\"ReadAt\" IS NULL")
            .HasDatabaseName("IX_NOTIFICATION_RECIPIENT_UNREAD");

        builder.HasIndex(x => x.SourceEventId)
            .IsUnique()
            .HasFilter("\"SourceEventId\" IS NOT NULL")
            .HasDatabaseName("UX_NOTIFICATION_SOURCE_EVENT");
    }
}
