namespace DoctorService.Application.Feature.DegreeType.Query.GetAll;

public sealed record GetAllDegreeTypesQuery(GetDegreeTypesRequest Request)
    : IQuery<Result<PaginatedResult<DegreeTypeResponse>>>;
