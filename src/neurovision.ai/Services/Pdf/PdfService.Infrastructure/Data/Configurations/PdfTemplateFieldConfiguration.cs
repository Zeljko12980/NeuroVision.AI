using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PdfService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfService.Infrastructure.Data.Configurations
{
    public class PdfTemplateFieldConfiguration
     : IEntityTypeConfiguration<PdfTemplateField>
    {
        public void Configure(
            EntityTypeBuilder<PdfTemplateField> builder)
        {
            builder.ToTable("pdf_template_fields");
            builder.HasKey(x => x.Id);


            builder.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();


            builder.Property(x => x.Type)
                .HasMaxLength(50)
                .IsRequired();



            builder.HasOne(x => x.PdfTemplate)
                .WithMany(x => x.Fields)
                .HasForeignKey(x => x.PdfTemplateId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
