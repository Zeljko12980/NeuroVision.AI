namespace DoctorService.Application.Feature.DoctorStatusHistory.Query.GetAll;

public sealed record GetAllDoctorStatusHistoriesQuery(GetDoctorStatusHistoriesRequest Request)
    : IQuery<Result<PaginatedResult<DoctorStatusHistoryResponse>>>;
