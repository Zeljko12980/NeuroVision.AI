using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TumorDetectionService.Application.Common.Interfaces;

namespace TumorDetectionService.Infrastructure.Services;

public class MlAnalysisOptions
{
    public string PythonExecutable { get; set; } = "python";
    public string ProjectRoot { get; set; } = string.Empty;
    public string PipelineScript { get; set; } = "scripts/test_pipeline.py";
    public string OutputsPath { get; set; } = "outputs";
    public string ArtifactsPath { get; set; } = "artifacts";
}

public class MlPythonAnalysisService : IMlAnalysisService
{
    private readonly MlAnalysisOptions _options;
    private readonly string _projectRoot;
    private readonly ILogger<MlPythonAnalysisService> _logger;

    public MlPythonAnalysisService(
        IOptions<MlAnalysisOptions> options,
        IWebHostEnvironment environment,
        ILogger<MlPythonAnalysisService> logger)
    {
        _options = options.Value;
        _logger = logger;
        _projectRoot = ResolvePath(_options.ProjectRoot, environment.ContentRootPath);
    }

    private static string ResolvePath(string configuredPath, string contentRoot)
    {
        if (string.IsNullOrWhiteSpace(configuredPath))
            return contentRoot;

        return Path.IsPathRooted(configuredPath)
            ? configuredPath
            : Path.GetFullPath(Path.Combine(contentRoot, configuredPath));
    }

    public async Task<MlPipelineResult> RunPipelineAsync(
        string imagePath,
        string? detectionRun,
        string? classificationRun,
        string? segmentationRun,
        CancellationToken cancellationToken = default)
    {
        var scriptPath = Path.Combine(_projectRoot, _options.PipelineScript);
        var args = new List<string> { scriptPath, imagePath };

        if (!string.IsNullOrWhiteSpace(detectionRun))
        {
            args.Add("--detection-run");
            args.Add(detectionRun);
        }
        if (!string.IsNullOrWhiteSpace(classificationRun))
        {
            args.Add("--classification-run");
            args.Add(classificationRun);
        }
        if (!string.IsNullOrWhiteSpace(segmentationRun))
        {
            args.Add("--segmentation-run");
            args.Add(segmentationRun);
        }

        var psi = new ProcessStartInfo
        {
            FileName = _options.PythonExecutable,
            WorkingDirectory = _projectRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        foreach (var arg in args)
            psi.ArgumentList.Add(arg);

        using var process = Process.Start(psi)
            ?? throw new InvalidOperationException("Failed to start ML pipeline process.");

        var stdout = await process.StandardOutput.ReadToEndAsync(cancellationToken);
        var stderr = await process.StandardError.ReadToEndAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken);

        if (process.ExitCode != 0)
        {
            _logger.LogError("ML pipeline failed: {Stderr}", stderr);
            throw new InvalidOperationException($"ML pipeline failed: {stderr}");
        }

        return ParseReport(imagePath, stdout, _projectRoot, _options.OutputsPath);
    }

    private static MlPipelineResult ParseReport(
        string imagePath,
        string stdout,
        string projectRoot,
        string outputsPath)
    {
        var reportPath = ResolveReportPath(stdout, projectRoot, outputsPath, imagePath);

        if (reportPath is null || !File.Exists(reportPath))
        {
            return new MlPipelineResult(
                Array.Empty<MlDetectionDto>(),
                null, 0, "{}", 0, null, null, null, 0);
        }

        using var doc = JsonDocument.Parse(File.ReadAllText(reportPath));
        var root = doc.RootElement;

        var detections = new List<MlDetectionDto>();
        if (root.TryGetProperty("detection", out var det) && det.TryGetProperty("boxes", out var boxes))
        {
            foreach (var box in boxes.EnumerateArray())
            {
                var bbox = box.GetProperty("bbox");
                detections.Add(new MlDetectionDto(
                    box.GetProperty("class").GetString() ?? "Unknown",
                    box.GetProperty("confidence").GetDouble(),
                    bbox[0].GetDouble(),
                    bbox[1].GetDouble(),
                    bbox[2].GetDouble(),
                    bbox[3].GetDouble()));
            }
        }

        string? cls = null;
        double clsConf = 0;
        var probs = "{}";
        if (root.TryGetProperty("classification", out var classification))
        {
            cls = classification.GetProperty("class").GetString();
            clsConf = classification.GetProperty("confidence").GetDouble();
            if (classification.TryGetProperty("probabilities", out var p))
                probs = p.GetRawText();
        }

        double area = 0;
        string? maskPath = null;
        if (root.TryGetProperty("segmentation", out var seg))
        {
            area = seg.GetProperty("tumor_area_ratio").GetDouble();
            if (seg.TryGetProperty("mask_path", out var maskProp)
                && maskProp.ValueKind == JsonValueKind.String)
            {
                maskPath = maskProp.GetString();
            }
        }

        var outputDir = Path.GetDirectoryName(reportPath)!;
        var imageFileName = Path.GetFileName(imagePath);
        var detectionAnnotated = Path.Combine(outputDir, "detection", imageFileName);
        var segmentationAnnotated = Path.Combine(outputDir, "segmentation", imageFileName);

        string? annotatedPath = null;
        if (File.Exists(detectionAnnotated))
            annotatedPath = detectionAnnotated;
        else if (File.Exists(segmentationAnnotated))
            annotatedPath = segmentationAnnotated;

        var overall = detections.Count > 0
            ? detections.Max(d => d.Confidence)
            : clsConf;

        return new MlPipelineResult(
            detections,
            cls,
            clsConf,
            probs,
            area,
            maskPath,
            annotatedPath,
            reportPath,
            overall);
    }

    private static string? ResolveReportPath(
        string stdout,
        string projectRoot,
        string outputsPath,
        string imagePath)
    {
        foreach (var line in stdout.Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            var trimmed = line.Trim();
            if (trimmed.StartsWith("Report:", StringComparison.OrdinalIgnoreCase))
                return trimmed["Report:".Length..].Trim();
        }

        var stem = Path.GetFileNameWithoutExtension(imagePath);
        var outputsRoot = Path.IsPathRooted(outputsPath)
            ? outputsPath
            : Path.Combine(projectRoot, outputsPath);
        var fallback = Path.Combine(outputsRoot, stem, "report.json");
        return Path.GetFullPath(fallback);
    }
}

