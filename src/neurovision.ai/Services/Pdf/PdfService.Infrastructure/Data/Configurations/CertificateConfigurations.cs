using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PdfService.Domain.Entities;

namespace PdfService.Infrastructure.Data.Configurations;

internal sealed class CertificateConfiguration : IEntityTypeConfiguration<Certificate>
{
    public void Configure(EntityTypeBuilder<Certificate> builder)
    {
        builder.ToTable("Certificates");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Subject)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Issuer)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Thumbprint)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.Thumbprint)
            .IsUnique();

        builder.Property(x => x.UserId);

        builder.HasIndex(x => x.UserId)
            .IsUnique();

        builder.Property(x => x.SignatureImagePath)
            .HasMaxLength(500);

        builder.Property(x => x.SerialNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ValidFrom)
            .IsRequired();

        builder.Property(x => x.ValidTo)
            .IsRequired();

        builder.Property(x => x.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.FilePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.IsDefault)
            .HasDefaultValue(false);

        builder.Property(x => x.ProtectedPassword)
            .IsRequired()
            .HasMaxLength(1000);
    }
}