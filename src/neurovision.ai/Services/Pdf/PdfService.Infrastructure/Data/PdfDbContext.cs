
using Microsoft.EntityFrameworkCore;
using PdfService.Domain.Entities;

namespace PdfService.Infrastructure.Data
{
    public class PdfDbContext:DbContext
    {
        public DbSet<PdfTemplate> Templates=> Set<PdfTemplate>();
        public DbSet<Certificate> Certificates => Set<Certificate>();
        public DbSet<PdfTemplateField> PdfTemplateFields { get; set; }
        public PdfDbContext(DbContextOptions<PdfDbContext> options):base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PdfDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
  
    }
}
