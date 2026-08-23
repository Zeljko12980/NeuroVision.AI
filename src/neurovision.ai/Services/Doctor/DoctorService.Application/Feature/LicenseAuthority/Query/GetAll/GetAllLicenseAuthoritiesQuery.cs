namespace DoctorService.Application.Feature.LicenseAuthority.Query.GetAll;

public sealed record GetAllLicenseAuthoritiesQuery(GetLicenseAuthoritiesRequest Request)
    : IQuery<Result<PaginatedResult<LicenseAuthorityResponse>>>;
