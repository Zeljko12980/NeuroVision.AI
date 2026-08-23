namespace DoctorService.Application.Feature.DoctorAffiliationHistory.Query.GetAll;

public sealed record GetAllDoctorAffiliationHistoriesQuery(GetDoctorAffiliationHistoriesRequest Request)
    : IQuery<Result<PaginatedResult<DoctorAffiliationHistoryResponse>>>;
