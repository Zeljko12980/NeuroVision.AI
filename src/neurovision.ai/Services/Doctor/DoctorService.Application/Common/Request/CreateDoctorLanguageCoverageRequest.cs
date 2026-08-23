namespace DoctorService.Application.Common.Request;

public sealed class CreateDoctorLanguageCoverageRequest
{
    public Guid DoctorId { get; set; }
    public string LanguageCode { get; set; } = null!;
}
