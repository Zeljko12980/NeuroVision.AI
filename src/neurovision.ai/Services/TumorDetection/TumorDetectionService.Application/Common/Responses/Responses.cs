namespace TumorDetectionService.Application.Common.Responses;

public record BrainScanResponse(
    Guid Id,
    Guid PatientId,
    string FileName,
    string ScanType,
    long FileSizeBytes,
    DateTime UploadedAt,
    int AnalysisCount);

public record DetectionFindingResponse(
    string ClassName,
    double Confidence,
    double XCenter,
    double YCenter,
    double Width,
    double Height);

public record AnalysisResponse(
    Guid Id,
    Guid BrainScanId,
    Guid PatientId,
    string ScanFileName,
    string Status,
    DateTime RequestedAt,
    DateTime? CompletedAt,
    double? OverallConfidence,
    string? ClassificationClass,
    double? ClassificationConfidence,
    double? TumorAreaRatio,
    IReadOnlyList<DetectionFindingResponse> Detections,
    string? ReportFilePath,
    bool HasAnnotatedImage,
    bool HasDetectionImage,
    bool HasSegmentationImage,
    bool HasMaskImage,
    bool HasPdfReport,
    DateTime? PdfGeneratedAt);

public record AnalysisReportResponse(
    Guid AnalysisId,
    Guid BrainScanId,
    Guid PatientId,
    string ScanFileName,
    string Status,
    DateTime? CompletedAt,
    DateTime? PdfGeneratedAt,
    string? ClassificationClass,
    double? OverallConfidence);

public record AnalysisStatisticsResponse(
    int TotalCompletedAnalyses,
    int TotalScans);

public record AiModelVersionResponse(
    Guid Id,
    string TaskType,
    string VersionLabel,
    string RunId,
    bool IsActive,
    DateTime RegisteredAt);

public record PaginatedResponse<T>(IReadOnlyList<T> Items, int Total, int Page, int PageSize);

public record AnalysisErrorLogResponse(
    Guid Id,
    Guid? TumorAnalysisId,
    string Message,
    string? Details,
    DateTime OccurredAt);

public record AnalysisFileResult(string FilePath, string? DownloadFileName);
