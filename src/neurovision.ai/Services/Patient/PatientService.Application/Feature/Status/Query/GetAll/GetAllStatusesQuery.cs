namespace PatientService.Application.Feature.Status.Query.GetAll;

public sealed record GetAllStatusesQuery(GetStatusesRequest Request)
    : IQuery<Result<PaginatedResult<PatientStatusResponse>>>;
