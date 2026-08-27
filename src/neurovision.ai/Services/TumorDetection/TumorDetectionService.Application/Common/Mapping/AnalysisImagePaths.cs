using TumorDetectionService.Domain.Entities;

namespace TumorDetectionService.Application.Common.Mapping;

public readonly record struct AnalysisImageAvailability(
    bool HasAnnotated,
    bool HasDetection,
    bool HasSegmentation,
    bool HasMask);

public static class AnalysisImagePaths
{
    public static AnalysisImageAvailability Resolve(TumorAnalysis analysis)
    {
        var detectionPath = ResolveFromReport(analysis, "detection");
        var segmentationPath = ResolveFromReport(analysis, "segmentation");
        var annotatedPath = analysis.Segmentation?.AnnotatedImagePath ?? detectionPath ?? segmentationPath;
        var maskPath = analysis.Segmentation?.MaskFilePath ?? ResolveMaskFromReport(analysis);

        return new AnalysisImageAvailability(
            HasAnnotated: FileExists(annotatedPath),
            HasDetection: FileExists(detectionPath),
            HasSegmentation: FileExists(segmentationPath),
            HasMask: FileExists(maskPath));
    }

    public static string? ResolveFilePath(TumorAnalysis analysis, string kind)
    {
        var scan = analysis.BrainScan;

        return kind.ToLowerInvariant() switch
        {
            "scan" => ExistingPath(scan.StoredFilePath),
            "annotated" => ExistingPath(
                analysis.Segmentation?.AnnotatedImagePath
                ?? ResolveFromReport(analysis, "detection")
                ?? ResolveFromReport(analysis, "segmentation")),
            "detection" => ExistingPath(ResolveFromReport(analysis, "detection")),
            "segmentation" => ExistingPath(ResolveFromReport(analysis, "segmentation")),
            "mask" => ExistingPath(analysis.Segmentation?.MaskFilePath ?? ResolveMaskFromReport(analysis)),
            _ => null
        };
    }

    private static string? ResolveFromReport(TumorAnalysis analysis, string folder)
    {
        if (string.IsNullOrWhiteSpace(analysis.ReportFilePath))
            return null;

        var outputDir = Path.GetDirectoryName(analysis.ReportFilePath);
        if (outputDir is null)
            return null;

        var imageFileName = Path.GetFileName(analysis.BrainScan.StoredFilePath);
        return Path.Combine(outputDir, folder, imageFileName);
    }

    private static string? ResolveMaskFromReport(TumorAnalysis analysis)
    {
        if (string.IsNullOrWhiteSpace(analysis.ReportFilePath))
            return null;

        try
        {
            using var doc = System.Text.Json.JsonDocument.Parse(File.ReadAllText(analysis.ReportFilePath));
            if (doc.RootElement.TryGetProperty("segmentation", out var seg)
                && seg.TryGetProperty("mask_path", out var maskProp)
                && maskProp.ValueKind == System.Text.Json.JsonValueKind.String)
            {
                return maskProp.GetString();
            }
        }
        catch
        {
            // ignore invalid report files
        }

        return null;
    }

    private static string? ExistingPath(string? path) =>
        FileExists(path) ? path : null;

    private static bool FileExists(string? path) =>
        !string.IsNullOrWhiteSpace(path) && File.Exists(path);
}
