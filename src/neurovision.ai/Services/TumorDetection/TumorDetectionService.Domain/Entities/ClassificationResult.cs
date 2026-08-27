using TumorDetectionService.Domain.Enums;

namespace TumorDetectionService.Domain.Entities;

public sealed class ClassificationResult
{
    public Guid Id { get; private set; }
    public Guid TumorAnalysisId { get; private set; }
    public TumorClassType PredictedClass { get; private set; }
    public double Confidence { get; private set; }
    public string ProbabilitiesJson { get; private set; } = "{}";

    public TumorAnalysis TumorAnalysis { get; private set; } = null!;

    private ClassificationResult() { }

    public static ClassificationResult Create(
        Guid tumorAnalysisId,
        TumorClassType predictedClass,
        double confidence,
        string probabilitiesJson)
    {
        return new ClassificationResult
        {
            Id = Guid.NewGuid(),
            TumorAnalysisId = tumorAnalysisId,
            PredictedClass = predictedClass,
            Confidence = confidence,
            ProbabilitiesJson = probabilitiesJson
        };
    }

    public void ApplyCorrection(TumorClassType correctedClass) => PredictedClass = correctedClass;
}
