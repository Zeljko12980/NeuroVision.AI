namespace DoctorService.Application.Feature.DoctorStatus.Query.GetAll;

public sealed record GetAllDoctorStatusesQuery(GetDoctorStatusesRequest Request)
    : IQuery<Result<PaginatedResult<DoctorStatusResponse>>>;
