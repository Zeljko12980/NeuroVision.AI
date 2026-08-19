using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.HealthInstitutionType.Query.GetAll
{
    public sealed record GetAllHealthInstitutionTypesQuery(GetHealthInstitutionTypesRequest Request) : IQuery<Result<PaginatedResult<HealthInstitutionTypeResponse>>>;
}
