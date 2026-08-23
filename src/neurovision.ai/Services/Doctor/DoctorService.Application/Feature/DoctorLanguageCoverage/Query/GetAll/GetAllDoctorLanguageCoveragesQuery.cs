namespace DoctorService.Application.Feature.DoctorLanguageCoverage.Query.GetAll;

public sealed record GetAllDoctorLanguageCoveragesQuery(GetDoctorLanguageCoveragesRequest Request)
    : IQuery<Result<PaginatedResult<DoctorLanguageCoverageResponse>>>;
