namespace PatientService.Application.Feature.PatientAllergyCoverage.Query.GetAll;

public sealed record GetAllPatientAllergyCoveragesQuery(GetPatientAllergyCoveragesRequest Request)
    : IQuery<Result<PaginatedResult<PatientAllergyCoverageResponse>>>;
