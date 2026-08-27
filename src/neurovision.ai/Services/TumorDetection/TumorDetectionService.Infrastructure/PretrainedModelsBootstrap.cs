using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TumorDetectionService.Infrastructure;

public static class PretrainedModelsBootstrap
{
    public static async Task EnsureDownloadedAsync(IConfiguration configuration, ILogger logger)
    {
        if (!configuration.GetValue("MlAnalysis:AutoDownloadPretrainedModels", true))
            return;

        var projectRoot = configuration["MlAnalysis:ProjectRoot"] ?? string.Empty;
        var artifactsRoot = configuration["MlAnalysis:ArtifactsPath"];

        if (string.IsNullOrWhiteSpace(artifactsRoot))
            artifactsRoot = Path.Combine(projectRoot, "artifacts");

        var detectionWeights = Path.Combine(
            artifactsRoot,
            "detection",
            configuration["MlAnalysis:SeedRuns:Detection"] ?? "hf_yolo11_brain_mri",
            "weights",
            "best.pt");
        var classificationWeights = Path.Combine(
            artifactsRoot,
            "classification",
            configuration["MlAnalysis:SeedRuns:Classification"] ?? "hf_yolov11_brain_cls",
            "weights",
            "best.pt");
        var segmentationWeights = Path.Combine(
            artifactsRoot,
            "segmentation",
            configuration["MlAnalysis:SeedRuns:Segmentation"] ?? "hf_yolo11_tumor_seg",
            "weights",
            "best.pt");

        if (File.Exists(detectionWeights)
            && File.Exists(classificationWeights)
            && File.Exists(segmentationWeights))
            return;

        var python = configuration["MlAnalysis:PythonExecutable"] ?? "python";
        var scriptPath = Path.Combine(projectRoot, "scripts", "download_pretrained_models.py");

        if (!File.Exists(scriptPath))
        {
            logger.LogWarning("Pretrained model download script not found at {ScriptPath}", scriptPath);
            return;
        }

        logger.LogInformation("Downloading pretrained Hugging Face models (first run)...");

        var psi = new ProcessStartInfo
        {
            FileName = python,
            WorkingDirectory = projectRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        psi.ArgumentList.Add(scriptPath);

        using var process = Process.Start(psi)
            ?? throw new InvalidOperationException("Failed to start pretrained model download.");

        var stdout = await process.StandardOutput.ReadToEndAsync();
        var stderr = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        if (!string.IsNullOrWhiteSpace(stdout))
            logger.LogInformation("{Output}", stdout.Trim());

        if (process.ExitCode != 0)
        {
            logger.LogError(
                "Pretrained model download failed (exit {ExitCode}): {Error}",
                process.ExitCode,
                stderr.Trim());
            return;
        }

        logger.LogInformation("Pretrained models downloaded successfully.");
    }
}
