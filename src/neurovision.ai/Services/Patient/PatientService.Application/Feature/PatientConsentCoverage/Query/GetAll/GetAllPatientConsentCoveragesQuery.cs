namespace PatientService.Application.Feature.PatientConsentCoverage.Query.GetAll;

public sealed record GetAllPatientConsentCoveragesQuery(GetPatientConsentCoveragesRequest Request)
    : IQuery<Result<PaginatedResult<PatientConsentCoverageResponse>>>;
