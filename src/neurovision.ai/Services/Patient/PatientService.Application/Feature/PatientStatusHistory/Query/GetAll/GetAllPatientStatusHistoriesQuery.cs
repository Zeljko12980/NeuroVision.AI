namespace PatientService.Application.Feature.PatientStatusHistory.Query.GetAll;

public sealed record GetAllPatientStatusHistoriesQuery(GetPatientStatusHistoriesRequest Request)
    : IQuery<Result<PaginatedResult<PatientStatusHistoryResponse>>>;
