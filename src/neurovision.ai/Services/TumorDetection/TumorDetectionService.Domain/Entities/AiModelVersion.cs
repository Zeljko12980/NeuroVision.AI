using TumorDetectionService.Domain.Enums;

namespace TumorDetectionService.Domain.Entities;

public sealed class AiModelVersion
{
    public Guid Id { get; private set; }
    public AiTaskType TaskType { get; private set; }
    public string VersionLabel { get; private set; } = string.Empty;
    public string RunId { get; private set; } = string.Empty;
    public string WeightsPath { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public Guid RegisteredByUserId { get; private set; }
    public DateTime RegisteredAt { get; private set; }

    private AiModelVersion() { }

    public static AiModelVersion Create(
        AiTaskType taskType,
        string versionLabel,
        string runId,
        string weightsPath,
        Guid registeredByUserId,
        bool isActive = false)
    {
        if (string.IsNullOrWhiteSpace(runId))
            throw new ArgumentException("RunId is required.", nameof(runId));

        return new AiModelVersion
        {
            Id = Guid.NewGuid(),
            TaskType = taskType,
            VersionLabel = versionLabel,
            RunId = runId,
            WeightsPath = weightsPath,
            IsActive = isActive,
            RegisteredByUserId = registeredByUserId,
            RegisteredAt = DateTime.UtcNow
        };
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    public void ReplaceRegistration(
        string versionLabel,
        string weightsPath,
        Guid registeredByUserId,
        bool isActive)
    {
        VersionLabel = versionLabel;
        WeightsPath = weightsPath;
        RegisteredByUserId = registeredByUserId;
        RegisteredAt = DateTime.UtcNow;
        IsActive = isActive;
    }
}
