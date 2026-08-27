namespace TumorDetectionService.Domain.Entities;

public sealed class AnalysisComment
{
    public Guid Id { get; private set; }
    public Guid TumorAnalysisId { get; private set; }
    public Guid AuthorUserId { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public TumorAnalysis TumorAnalysis { get; private set; } = null!;

    private AnalysisComment() { }

    public static AnalysisComment Create(Guid tumorAnalysisId, Guid authorUserId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content is required.", nameof(content));

        return new AnalysisComment
        {
            Id = Guid.NewGuid(),
            TumorAnalysisId = tumorAnalysisId,
            AuthorUserId = authorUserId,
            Content = content.Trim(),
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content is required.", nameof(content));

        Content = content.Trim();
        UpdatedAt = DateTime.UtcNow;
    }
}
