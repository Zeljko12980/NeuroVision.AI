namespace DoctorService.Application.Feature.DoctorSpecializationCoverage.Query.GetAll;

public sealed record GetAllDoctorSpecializationCoveragesQuery(GetDoctorSpecializationCoveragesRequest Request)
    : IQuery<Result<PaginatedResult<DoctorSpecializationCoverageResponse>>>;
