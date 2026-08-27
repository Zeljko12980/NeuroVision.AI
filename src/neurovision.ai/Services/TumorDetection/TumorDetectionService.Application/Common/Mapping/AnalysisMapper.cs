using TumorDetectionService.Application.Common.Responses;
using TumorDetectionService.Domain.Entities;

namespace TumorDetectionService.Application.Common.Mapping;

public static class AnalysisMapper
{
    public static AnalysisResponse ToResponse(TumorAnalysis analysis)
    {
        var imagePaths = AnalysisImagePaths.Resolve(analysis);

        return new(
            analysis.Id,
            analysis.BrainScanId,
            analysis.BrainScan.PatientId,
            analysis.BrainScan.FileName,
            analysis.Status.ToString(),
            analysis.RequestedAt,
            analysis.CompletedAt,
            analysis.OverallConfidence,
            analysis.Classification?.PredictedClass.ToString(),
            analysis.Classification?.Confidence,
            analysis.Segmentation?.TumorAreaRatio,
            analysis.Detections
                .Select(d => new DetectionFindingResponse(
                    d.ClassName, d.Confidence, d.XCenter, d.YCenter, d.Width, d.Height))
                .ToList(),
            analysis.ReportFilePath,
            imagePaths.HasAnnotated,
            imagePaths.HasDetection,
            imagePaths.HasSegmentation,
            imagePaths.HasMask,
            !string.IsNullOrWhiteSpace(analysis.PdfReportPath) && File.Exists(analysis.PdfReportPath),
            analysis.PdfGeneratedAt);
    }
}
