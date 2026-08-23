namespace PatientService.Application.Feature.BloodType.Query.GetAll;

public sealed record GetAllBloodTypesQuery(GetBloodTypesRequest Request)
    : IQuery<Result<PaginatedResult<BloodTypeResponse>>>;
