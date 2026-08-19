using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.HealthInstitution.Query.GetAll
{
    public sealed record GetAllHealthInstitutionsQuery(GetHealthInstitutionsRequest Request) : IQuery<Result<PaginatedResult<HealthInstitutionResponse>>>;
}
