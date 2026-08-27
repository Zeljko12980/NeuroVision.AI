using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using TumorDetectionService.Application.Common.Interfaces;
using TumorDetectionService.Domain.Enums;

namespace TumorDetectionService.Infrastructure.Services;

public sealed class ModelStorageService : IModelStorageService
{
    private readonly string _artifactsRoot;

    public ModelStorageService(IOptions<MlAnalysisOptions> options, IWebHostEnvironment environment)
    {
        var configured = options.Value;
        var projectRoot = ResolvePath(configured.ProjectRoot, environment.ContentRootPath);
        _artifactsRoot = ResolvePath(configured.ArtifactsPath, projectRoot);
        Directory.CreateDirectory(_artifactsRoot);
    }

    public async Task<string> SaveWeightsAsync(
        AiTaskType taskType,
        string runId,
        Stream content,
        CancellationToken cancellationToken = default)
    {
        var folder = taskType switch
        {
            AiTaskType.Detection => "detection",
            AiTaskType.Classification => "classification",
            AiTaskType.Segmentation => "segmentation",
            _ => throw new ArgumentOutOfRangeException(nameof(taskType), taskType, "Unknown AI task type.")
        };

        var safeRunId = SanitizeRunId(runId);
        var destDir = Path.Combine(_artifactsRoot, folder, safeRunId, "weights");
        Directory.CreateDirectory(destDir);

        var destPath = Path.Combine(destDir, "best.pt");
        await using var file = File.Create(destPath);
        await content.CopyToAsync(file, cancellationToken);
        return destPath;
    }

    private static string SanitizeRunId(string runId)
    {
        var trimmed = runId.Trim();
        var chars = trimmed
            .Select(ch => char.IsLetterOrDigit(ch) || ch is '-' or '_' ? ch : '_')
            .ToArray();
        var sanitized = new string(chars);
        return string.IsNullOrWhiteSpace(sanitized) ? $"upload_{Guid.NewGuid():N}" : sanitized;
    }

    private static string ResolvePath(string configuredPath, string basePath)
    {
        if (string.IsNullOrWhiteSpace(configuredPath))
            return basePath;

        return Path.IsPathRooted(configuredPath)
            ? configuredPath
            : Path.GetFullPath(Path.Combine(basePath, configuredPath));
    }
}
