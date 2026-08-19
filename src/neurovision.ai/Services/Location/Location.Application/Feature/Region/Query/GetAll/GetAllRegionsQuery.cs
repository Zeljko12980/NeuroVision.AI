using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.Region.Query.GetAll
{
    public sealed record GetAllRegionsQuery(GetRegionsRequest Request) : IQuery<Result<PaginatedResult<RegionResponse>>>;
}
