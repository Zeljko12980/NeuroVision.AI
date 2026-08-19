using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PdfService.Domain.Entities;

namespace PdfService.Infrastructure.Data;

public static class PdfDbinitializer
{
    public static async Task InitializeAsync(
        IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PdfDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<PdfDbContext>>();

        var pending = await db.Database.GetPendingMigrationsAsync(cancellationToken);
        if (pending.Any())
        {
            logger.LogInformation(
                "Applying database migrations: {Migrations}",
                string.Join(", ", pending));
        }

        await db.Database.MigrateAsync(cancellationToken);

        await PdfTemplateSeeder.SeedAsync(db, logger, cancellationToken);

        try
        {
            await EnsureTumorAnalysisReportTemplateAsync(db, logger, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Failed to ensure TUMOR_ANALYSIS_REPORT template; PDF service will continue startup.");
        }

        await PdfCertificateSeeder.SeedAsync(services, cancellationToken);
    }

    private static async Task EnsureTumorAnalysisReportTemplateAsync(
        PdfDbContext db,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        const string code = PdfSeedConstants.TumorAnalysisReport;

        var templateId = await db.Templates
            .AsNoTracking()
            .Where(x => x.Code == code)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (templateId == Guid.Empty)
            return;

        await db.Templates
            .Where(x => x.Id == templateId)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(t => t.RequiresSignature, true)
                    .SetProperty(t => t.SignaturePage, 1)
                    .SetProperty(t => t.IsActive, true),
                cancellationToken);

        var signatureFieldsUpdated = await db.PdfTemplateFields
            .Where(f => f.PdfTemplateId == templateId && f.Type == "Signature")
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(f => f.Page, 0)
                    .SetProperty(f => f.X, 72f)
                    .SetProperty(f => f.Y, 72f)
                    .SetProperty(f => f.Width, 220f)
                    .SetProperty(f => f.Height, 70f),
                cancellationToken);

        if (signatureFieldsUpdated == 0)
        {
            await db.PdfTemplateFields.AddAsync(
                PdfTemplateField.Create(
                    name: "Signature",
                    type: "Signature",
                    page: 0,
                    x: 72,
                    y: 72,
                    width: 220,
                    height: 70,
                    pdfTemplateId: templateId),
                cancellationToken);

            await db.SaveChangesAsync(cancellationToken);
        }

        logger.LogInformation("Ensured tumor analysis report template with digital signature support.");
    }
}
