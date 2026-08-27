namespace TumorDetectionService.Application.Common.Interfaces;

public interface IPdfReportGenerator
{
    Task<PdfReportGenerationResult> GenerateTumorAnalysisReportAsync(
        Dictionary<string, string> templateData,
        Guid? certificateId = null,
        CancellationToken cancellationToken = default);
}

public sealed record PdfReportGenerationResult(byte[] PdfBytes, bool IsSigned);

public interface IReportStorageService
{
    Task<string> SaveReportAsync(
        Guid analysisId,
        byte[] pdfBytes,
        CancellationToken cancellationToken = default);

    string? GetReportPath(Guid analysisId);
}

public static class TumorReportTemplates
{
    public const string AnalysisReport = "TUMOR_ANALYSIS_REPORT";
}
