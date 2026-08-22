using DoctorService.Domain.Entities;

namespace DoctorService.Infrastructure.Persistence.Configurations;

public class DoctorReviewConfiguration : IEntityTypeConfiguration<DoctorReview>
{
    public void Configure(EntityTypeBuilder<DoctorReview> builder)
    {
        builder.ToTable("DoctorReviews");

        builder.HasKey(x => new { x.DoctorId, x.SequenceNumber })
            .HasName("PK_DOCTOR_REVIEW");

        builder.Property(x => x.DoctorId)
            .HasColumnName("DoctorId")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(x => x.SequenceNumber)
            .HasColumnName("SequenceNumber")
            .HasColumnType("numeric(2,0)")
            .IsRequired();

        builder.Property(x => x.Rating)
            .HasColumnName("Rating")
            .HasColumnType("numeric(2,1)")
            .IsRequired();

        builder.Property(x => x.Comment)
            .HasColumnName("Comment")
            .HasColumnType("varchar(2000)");

        builder.Property(x => x.ReviewerUserId)
            .HasColumnName("ReviewerUserId")
            .HasColumnType("uuid");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("timestamp")
            .IsRequired();

        builder.HasOne(x => x.Doctor)
            .WithMany(d => d.Reviews)
            .HasForeignKey(x => x.DoctorId)
            .HasConstraintName("FK_DOCTOR_REVIEW_DOCTOR")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.DoctorId)
            .HasDatabaseName("IX_DOCTOR_REVIEW_DOCTOR");
    }
}
