namespace DoctorService.Application.Common.Response;

public class DoctorCatalogsResponse
{
    public IReadOnlyList<SpecializationResponse> Specializations { get; init; } = [];
    public IReadOnlyList<LanguageResponse> Languages { get; init; } = [];
    public IReadOnlyList<DegreeTypeResponse> DegreeTypes { get; init; } = [];
    public IReadOnlyList<LicenseAuthorityResponse> LicenseAuthorities { get; init; } = [];
}
