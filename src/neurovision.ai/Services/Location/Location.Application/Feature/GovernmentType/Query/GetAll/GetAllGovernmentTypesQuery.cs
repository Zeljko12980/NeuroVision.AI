using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.GovernmentType.Query.GetAll
{
    public sealed record GetAllGovernmentTypesQuery(GetGovernmentTypesRequest Request) : IQuery<Result<PaginatedResult<GovernmentTypeResponse>>>;
}
