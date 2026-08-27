namespace TumorDetectionService.Domain.Entities;

public sealed class SegmentationResult
{
    public Guid Id { get; private set; }
    public Guid TumorAnalysisId { get; private set; }
    public double TumorAreaRatio { get; private set; }
    public string? MaskFilePath { get; private set; }
    public string? AnnotatedImagePath { get; private set; }

    public TumorAnalysis TumorAnalysis { get; private set; } = null!;

    private SegmentationResult() { }

    public static SegmentationResult Create(
        Guid tumorAnalysisId,
        double tumorAreaRatio,
        string? maskFilePath,
        string? annotatedImagePath)
    {
        return new SegmentationResult
        {
            Id = Guid.NewGuid(),
            TumorAnalysisId = tumorAnalysisId,
            TumorAreaRatio = tumorAreaRatio,
            MaskFilePath = maskFilePath,
            AnnotatedImagePath = annotatedImagePath
        };
    }
}
