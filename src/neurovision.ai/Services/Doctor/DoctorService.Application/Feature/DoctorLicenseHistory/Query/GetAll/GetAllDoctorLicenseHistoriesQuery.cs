namespace DoctorService.Application.Feature.DoctorLicenseHistory.Query.GetAll;

public sealed record GetAllDoctorLicenseHistoriesQuery(GetDoctorLicenseHistoriesRequest Request)
    : IQuery<Result<PaginatedResult<DoctorLicenseHistoryResponse>>>;
