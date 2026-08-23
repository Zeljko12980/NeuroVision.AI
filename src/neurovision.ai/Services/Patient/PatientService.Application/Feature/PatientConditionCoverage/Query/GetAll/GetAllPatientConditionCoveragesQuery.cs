namespace PatientService.Application.Feature.PatientConditionCoverage.Query.GetAll;

public sealed record GetAllPatientConditionCoveragesQuery(GetPatientConditionCoveragesRequest Request)
    : IQuery<Result<PaginatedResult<PatientConditionCoverageResponse>>>;
