using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.RegionType.Query.GetAll
{
    public sealed record GetAllRegionTypesQuery(GetRegionTypesRequest Request) : IQuery<Result<PaginatedResult<RegionTypeResponse>>>;
}
