namespace PatientService.Application.Feature.PatientLanguageCoverage.Query.GetAll;

public sealed record GetAllPatientLanguageCoveragesQuery(GetPatientLanguageCoveragesRequest Request)
    : IQuery<Result<PaginatedResult<PatientLanguageCoverageResponse>>>;
