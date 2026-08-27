using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TumorDetectionService.Domain.Entities;
using TumorDetectionService.Domain.Enums;
using TumorDetectionService.Infrastructure.Persistence;

namespace TumorDetectionService.Infrastructure;

public static class TumorDetectionDataSeeder
{
    private static readonly Guid SystemUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public static async Task SeedModelTypesAsync(TumorDetectionDbContext context, ILogger logger)
    {
        await context.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS ai_model_types (
                "Code" character varying(50) NOT NULL,
                "Name" character varying(100) NOT NULL,
                "Description" character varying(500) NULL,
                CONSTRAINT "PK_ai_model_types" PRIMARY KEY ("Code")
            );
            """);

        var required = new (string Code, string Name, string Description)[]
        {
            (nameof(AiTaskType.Detection), "Detection", "YOLO object detection of tumors on the MRI slice."),
            (nameof(AiTaskType.Classification), "Classification", "YOLO image classification of tumor type."),
            (nameof(AiTaskType.Segmentation), "Segmentation", "YOLO instance segmentation of tumor area.")
        };

        var existing = await context.AiModelTypes.Select(x => x.Code).ToListAsync();
        var added = 0;
        foreach (var (code, name, description) in required)
        {
            if (existing.Contains(code, StringComparer.OrdinalIgnoreCase))
                continue;

            await context.AiModelTypes.AddAsync(AiModelType.Create(code, name, description));
            added++;
        }

        if (added == 0)
            return;

        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} AI model type(s).", added);
    }

    public static async Task SeedClinicalCatalogsAsync(TumorDetectionDbContext context, ILogger logger)
    {
        await context.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS clinical_catalog_items (
                "Category" integer NOT NULL,
                "Code" character varying(50) NOT NULL,
                "Name" character varying(100) NOT NULL,
                "Description" character varying(500) NULL,
                CONSTRAINT "PK_clinical_catalog_items" PRIMARY KEY ("Category", "Code")
            );
            """);

        var required = new (ClinicalCatalogCategory Category, string Code, string Name, string Description)[]
        {
            (ClinicalCatalogCategory.Grade, "G1", "WHO Grade I", "Low-grade, slow-growing tumor."),
            (ClinicalCatalogCategory.Grade, "G2", "WHO Grade II", "Low-to-intermediate grade."),
            (ClinicalCatalogCategory.Grade, "G3", "WHO Grade III", "High-grade, malignant."),
            (ClinicalCatalogCategory.Grade, "G4", "WHO Grade IV", "Highest grade, aggressive."),
            (ClinicalCatalogCategory.Grade, "PENDING", "Pending histology", "Final grade awaits biopsy or surgery."),
            (ClinicalCatalogCategory.Operability, "OPERABLE", "Operable", "Tumor can likely be removed safely."),
            (ClinicalCatalogCategory.Operability, "PARTIAL", "Partially operable", "Subtotal resection may be possible."),
            (ClinicalCatalogCategory.Operability, "NOT_OPERABLE", "Not operable", "Surgery is not considered safe."),
            (ClinicalCatalogCategory.Operability, "UNKNOWN", "Unknown", "Operability not yet assessed."),
            (ClinicalCatalogCategory.Spread, "LOCAL", "Localized", "No evidence of spread beyond primary site."),
            (ClinicalCatalogCategory.Spread, "REGIONAL", "Regional spread", "Local or regional extension suspected."),
            (ClinicalCatalogCategory.Spread, "METASTATIC", "Metastatic", "Distant spread suspected or confirmed."),
            (ClinicalCatalogCategory.Spread, "UNKNOWN", "Unknown", "Spread status not yet assessed."),
            (ClinicalCatalogCategory.TreatmentOption, "SURG", "Surgery", "Resection when accessible and safe; tissue sample for histopathology."),
            (ClinicalCatalogCategory.TreatmentOption, "RT", "Radiotherapy", "Often after surgery for certain tumor types."),
            (ClinicalCatalogCategory.TreatmentOption, "CHEMO", "Chemotherapy", "Drug therapy depending on tumor biology."),
            (ClinicalCatalogCategory.TreatmentOption, "TARGETED", "Targeted / immunotherapy", "When molecular findings suggest benefit."),
            (ClinicalCatalogCategory.TreatmentOption, "SYMPTOMS", "Symptom treatment", "For example swelling or seizure control."),
            (ClinicalCatalogCategory.TreatmentOption, "WATCH", "Watchful waiting", "Regular MRI for small, slow-growing, asymptomatic tumors.")
        };

        var existing = await context.ClinicalCatalogItems
            .Select(x => new { x.Category, x.Code })
            .ToListAsync();

        var added = 0;
        foreach (var (category, code, name, description) in required)
        {
            if (existing.Any(x => x.Category == category && x.Code == code))
                continue;

            await context.ClinicalCatalogItems.AddAsync(
                ClinicalCatalogItem.Create(category, code, name, description));
            added++;
        }

        if (added == 0)
            return;

        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} clinical catalog item(s).", added);
    }

    public static async Task SeedAsync(
        TumorDetectionDbContext context,
        IConfiguration configuration,
        ILogger logger)
    {
        if (await context.AiModelVersions.AnyAsync())
            return;

        var projectRoot = configuration["MlAnalysis:ProjectRoot"] ?? string.Empty;
        var artifactsRoot = configuration["MlAnalysis:ArtifactsPath"];

        if (string.IsNullOrWhiteSpace(artifactsRoot))
            artifactsRoot = Path.Combine(projectRoot, "artifacts");

        var seeded = 0;

        seeded += await SeedTaskModelAsync(
            context,
            configuration,
            artifactsRoot,
            AiTaskType.Detection,
            "detection",
            logger);

        seeded += await SeedTaskModelAsync(
            context,
            configuration,
            artifactsRoot,
            AiTaskType.Classification,
            "classification",
            logger);

        seeded += await SeedTaskModelAsync(
            context,
            configuration,
            artifactsRoot,
            AiTaskType.Segmentation,
            "segmentation",
            logger);

        if (seeded > 0)
        {
            await context.SaveChangesAsync();
            logger.LogInformation("Seeded {Count} AI model version(s).", seeded);
        }
        else
        {
            logger.LogWarning(
                "No AI model runs found under {ArtifactsRoot}. Train models or set MlAnalysis:SeedRuns in appsettings.",
                artifactsRoot);
        }
    }

    private static async Task<int> SeedTaskModelAsync(
        TumorDetectionDbContext context,
        IConfiguration configuration,
        string artifactsRoot,
        AiTaskType taskType,
        string folderName,
        ILogger logger)
    {
        var configuredRunId = configuration[$"MlAnalysis:SeedRuns:{taskType}"];
        var runId = configuredRunId ?? DiscoverLatestRun(Path.Combine(artifactsRoot, folderName));

        if (string.IsNullOrWhiteSpace(runId))
            return 0;

        var weightsPath = Path.Combine(artifactsRoot, folderName, runId, "weights");
        if (!Directory.Exists(weightsPath))
        {
            logger.LogWarning("Weights folder not found for {TaskType} run {RunId}.", taskType, runId);
            return 0;
        }

        await context.AiModelVersions.AddAsync(
            AiModelVersion.Create(
                taskType,
                ResolveLabel(configuration, taskType),
                runId,
                weightsPath,
                SystemUserId,
                isActive: true));

        return 1;
    }

    private static string? DiscoverLatestRun(string taskDirectory)
    {
        if (!Directory.Exists(taskDirectory))
            return null;

        return Directory.GetDirectories(taskDirectory)
            .Select(Path.GetFileName)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .OrderByDescending(name => name, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault(runId =>
            {
                var weightsDir = Path.Combine(taskDirectory, runId!, "weights");
                if (!Directory.Exists(weightsDir))
                    return false;

                return Directory.EnumerateFiles(weightsDir).Any();
            });
    }

    private static string ResolveLabel(IConfiguration configuration, AiTaskType taskType)
    {
        var configured = configuration[$"MlAnalysis:PretrainedModels:{taskType}:Label"];
        return string.IsNullOrWhiteSpace(configured) ? $"Default {taskType}" : configured;
    }
}
