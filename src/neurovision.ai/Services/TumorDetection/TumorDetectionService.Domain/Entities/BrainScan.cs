using TumorDetectionService.Domain.Enums;

namespace TumorDetectionService.Domain.Entities;

public sealed class BrainScan
{
    public Guid Id { get; private set; }
    public Guid PatientId { get; private set; }
    public Guid UploadedByUserId { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public string StoredFilePath { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public ScanType ScanType { get; private set; }
    public long FileSizeBytes { get; private set; }
    public DateTime UploadedAt { get; private set; }

    public ICollection<TumorAnalysis> Analyses { get; private set; } = new List<TumorAnalysis>();

    private BrainScan() { }

    public static BrainScan Create(
        Guid patientId,
        Guid uploadedByUserId,
        string fileName,
        string storedFilePath,
        string contentType,
        ScanType scanType,
        long fileSizeBytes,
        Guid? id = null)
    {
        if (patientId == Guid.Empty)
            throw new ArgumentException("PatientId is required.", nameof(patientId));
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("FileName is required.", nameof(fileName));
        if (string.IsNullOrWhiteSpace(storedFilePath))
            throw new ArgumentException("StoredFilePath is required.", nameof(storedFilePath));

        return new BrainScan
        {
            Id = id ?? Guid.NewGuid(),
            PatientId = patientId,
            UploadedByUserId = uploadedByUserId,
            FileName = fileName,
            StoredFilePath = storedFilePath,
            ContentType = contentType,
            ScanType = scanType,
            FileSizeBytes = fileSizeBytes,
            UploadedAt = DateTime.UtcNow
        };
    }
}
