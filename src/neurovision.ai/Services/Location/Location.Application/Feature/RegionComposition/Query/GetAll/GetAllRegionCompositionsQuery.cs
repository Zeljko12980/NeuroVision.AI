using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.RegionComposition.Query.GetAll
{
    public sealed record GetAllRegionCompositionsQuery(GetRegionCompositionsRequest Request) : IQuery<Result<PaginatedResult<RegionCompositionResponse>>>;
}
