namespace DoctorService.Application.Common.Response;

public class DoctorLanguageCoverageResponse
{
    public Guid DoctorId { get; set; }
    public string LanguageCode { get; set; } = null!;
}
