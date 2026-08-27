using BuildingBlocks.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TumorDetectionService.Application.Common.Interfaces;
using TumorDetectionService.Domain.Enums;

namespace TumorDetectionService.Infrastructure.Services;

public sealed class AnalysisJobRunner : IAnalysisJobRunner
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AnalysisJobRunner> _logger;

    public AnalysisJobRunner(IServiceScopeFactory scopeFactory, ILogger<AnalysisJobRunner> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public void StartAnalysisJob(
        Guid analysisId,
        string scanFilePath,
        string? detectionRun,
        string? classificationRun,
        string? segmentationRun)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await ProcessAsync(
                    analysisId,
                    scanFilePath,
                    detectionRun,
                    classificationRun,
                    segmentationRun);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error while running analysis {AnalysisId}", analysisId);
            }
        });
    }

    private async Task ProcessAsync(
        Guid analysisId,
        string scanFilePath,
        string? detectionRun,
        string? classificationRun,
        string? segmentationRun)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();

        var analyses = scope.ServiceProvider.GetRequiredService<ITumorAnalysisRepository>();
        var ml = scope.ServiceProvider.GetRequiredService<IMlAnalysisService>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var publisher = scope.ServiceProvider.GetRequiredService<IAnalysisNotificationPublisher>();
        var inbox = scope.ServiceProvider.GetRequiredService<ITumorInboxNotifications>();

        try
        {
            var result = await ml.RunPipelineAsync(
                scanFilePath,
                detectionRun,
                classificationRun,
                segmentationRun);

            var predictedClass = string.IsNullOrWhiteSpace(result.ClassificationClass)
                ? (TumorClassType?)null
                : MapClass(result.ClassificationClass);

            if (predictedClass is null && result.Detections.Count > 0)
            {
                var topDetection = result.Detections
                    .OrderByDescending(d => d.Confidence)
                    .First();
                predictedClass = MapClass(topDetection.ClassName);
            }

            await analyses.ApplyPipelineResultsAsync(analysisId, result, predictedClass);
            await unitOfWork.SaveChangesAsync();

            var completed = await analyses.GetByIdWithDetailsAsync(analysisId);
            if (completed is not null)
            {
                await publisher.PublishAsync(new AnalysisStatusNotification(
                    completed.Id,
                    completed.BrainScanId,
                    completed.BrainScan.PatientId,
                    completed.Status.ToString()));
                await inbox.PublishAnalysisOutcomeAsync(completed, succeeded: true, errorMessage: null);
            }
        }
        catch (Exception ex)
        {
            await analyses.ApplyFailureAsync(analysisId, ex.Message, ex.ToString());
            await unitOfWork.SaveChangesAsync();

            var failed = await analyses.GetByIdWithDetailsAsync(analysisId);
            if (failed is not null)
            {
                await publisher.PublishAsync(new AnalysisStatusNotification(
                    failed.Id,
                    failed.BrainScanId,
                    failed.BrainScan.PatientId,
                    failed.Status.ToString()));
                await inbox.PublishAnalysisOutcomeAsync(failed, succeeded: false, ex.Message);
            }
        }
    }

    private static TumorClassType? MapClass(string className)
    {
        var key = className.Trim().ToLowerInvariant().Replace("_", " ").Replace("-", " ");
        key = string.Join(' ', key.Split(' ', StringSplitOptions.RemoveEmptyEntries));

        return key switch
        {
            "glioma" or "glioma tumor" => TumorClassType.Glioma,
            "meningioma" or "meningioma tumor" => TumorClassType.Meningioma,
            "pituitary" or "pituitary tumor" => TumorClassType.Pituitary,
            "no tumor" or "notumor" or "healthy" => TumorClassType.NoTumor,
            _ => null
        };
    }
}
