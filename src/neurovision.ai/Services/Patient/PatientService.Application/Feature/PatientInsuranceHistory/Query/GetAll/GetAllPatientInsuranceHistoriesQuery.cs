namespace PatientService.Application.Feature.PatientInsuranceHistory.Query.GetAll;

public sealed record GetAllPatientInsuranceHistoriesQuery(GetPatientInsuranceHistoriesRequest Request)
    : IQuery<Result<PaginatedResult<PatientInsuranceHistoryResponse>>>;
