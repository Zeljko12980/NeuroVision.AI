namespace TumorDetectionService.Application.Common.Options;

public sealed class PdfServiceOptions
{
    public string GrpcUrl { get; set; } = "http://localhost:6102";

    /// <summary>gRPC call timeout in seconds (PDF generation + signing can be slow).</summary>
    public int TimeoutSeconds { get; set; } = 180;

    public Guid? DefaultCertificateId { get; set; }

    public string DefaultDoctorName { get; set; } = "Attending Physician";
}
