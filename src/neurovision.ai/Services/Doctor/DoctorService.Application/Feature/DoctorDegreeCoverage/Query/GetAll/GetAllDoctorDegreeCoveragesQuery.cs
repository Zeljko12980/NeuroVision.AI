namespace DoctorService.Application.Feature.DoctorDegreeCoverage.Query.GetAll;

public sealed record GetAllDoctorDegreeCoveragesQuery(GetDoctorDegreeCoveragesRequest Request)
    : IQuery<Result<PaginatedResult<DoctorDegreeCoverageResponse>>>;
