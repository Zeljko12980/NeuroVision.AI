namespace NotificationService.Infrastructure.Persistence.Configurations;

public class NotificationPreferenceConfiguration : IEntityTypeConfiguration<NotificationPreference>
{
    public void Configure(EntityTypeBuilder<NotificationPreference> builder)
    {
        builder.ToTable("NotificationPreferences");

        builder.HasKey(x => new { x.UserId, x.TypeCode, x.ChannelCode })
            .HasName("PK_NOTIFICATION_PREFERENCE");

        builder.Property(x => x.UserId)
            .HasColumnName("UserId")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(x => x.TypeCode)
            .HasColumnName("TypeCode")
            .HasColumnType("varchar(10)")
            .IsRequired();

        builder.Property(x => x.ChannelCode)
            .HasColumnName("ChannelCode")
            .HasColumnType("varchar(10)")
            .IsRequired();

        builder.Property(x => x.Enabled)
            .HasColumnName("Enabled")
            .HasColumnType("boolean")
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasOne(x => x.Type)
            .WithMany(t => t.Preferences)
            .HasForeignKey(x => x.TypeCode)
            .HasConstraintName("FK_NOTIFICATION_PREFERENCE_TYPE")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Channel)
            .WithMany(c => c.Preferences)
            .HasForeignKey(x => x.ChannelCode)
            .HasConstraintName("FK_NOTIFICATION_PREFERENCE_CHANNEL")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.TypeCode)
            .HasDatabaseName("IX_NOTIFICATION_PREFERENCE_TYPE");

        builder.HasIndex(x => x.ChannelCode)
            .HasDatabaseName("IX_NOTIFICATION_PREFERENCE_CHANNEL");
    }
}
