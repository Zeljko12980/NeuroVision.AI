namespace TumorDetectionService.Domain.Entities;

public sealed class DetectionFinding
{
    public Guid Id { get; private set; }
    public Guid TumorAnalysisId { get; private set; }
    public string ClassName { get; private set; } = string.Empty;
    public double Confidence { get; private set; }
    public double XCenter { get; private set; }
    public double YCenter { get; private set; }
    public double Width { get; private set; }
    public double Height { get; private set; }

    public TumorAnalysis TumorAnalysis { get; private set; } = null!;

    private DetectionFinding() { }

    public static DetectionFinding Create(
        Guid tumorAnalysisId,
        string className,
        double confidence,
        double xCenter,
        double yCenter,
        double width,
        double height)
    {
        return new DetectionFinding
        {
            Id = Guid.NewGuid(),
            TumorAnalysisId = tumorAnalysisId,
            ClassName = className,
            Confidence = confidence,
            XCenter = xCenter,
            YCenter = yCenter,
            Width = width,
            Height = height
        };
    }
}
