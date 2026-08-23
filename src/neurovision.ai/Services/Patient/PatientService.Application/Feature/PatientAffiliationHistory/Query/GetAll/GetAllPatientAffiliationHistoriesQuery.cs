namespace PatientService.Application.Feature.PatientAffiliationHistory.Query.GetAll;

public sealed record GetAllPatientAffiliationHistoriesQuery(GetPatientAffiliationHistoriesRequest Request)
    : IQuery<Result<PaginatedResult<PatientAffiliationHistoryResponse>>>;
