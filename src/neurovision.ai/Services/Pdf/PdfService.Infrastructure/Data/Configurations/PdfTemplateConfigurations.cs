using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PdfService.Domain.Entities;

namespace PdfService.Infrastructure.Data.Configurations
{
    public sealed class PdfTemplateConfigurations
        : IEntityTypeConfiguration<PdfTemplate>
    {
        public void Configure(EntityTypeBuilder<PdfTemplate> builder)
        {
            builder.ToTable("pdf_templates");


            builder.HasKey(x => x.Id);


            builder.Property(x => x.Code)
                .HasMaxLength(64)
                .IsRequired();


            builder.HasIndex(x => x.Code)
                .IsUnique();


            builder.Property(x => x.Name)
                .HasMaxLength(256)
                .IsRequired();


            builder.Property(x => x.HtmlContent)
                .IsRequired();



            builder.Property(x => x.Version)
                .IsRequired();


            builder.Property(x => x.IsActive)
                .IsRequired();



            builder.Property(x => x.CreatedAt)
                .IsRequired();


            builder.Property(x => x.UpdatedAt);




            builder.Property(x => x.RequiresSignature)
                .IsRequired()
                .HasDefaultValue(false);


            builder.Property(x => x.SignaturePage)
                .IsRequired()
                .HasDefaultValue(1);



            builder.HasMany(x => x.Fields)
                .WithOne(x => x.PdfTemplate)
                .HasForeignKey(x => x.PdfTemplateId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}