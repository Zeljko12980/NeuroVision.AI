namespace PatientService.Application.Feature.Gender.Query.GetAll;

public sealed record GetAllGendersQuery(GetGendersRequest Request)
    : IQuery<Result<PaginatedResult<GenderResponse>>>;
