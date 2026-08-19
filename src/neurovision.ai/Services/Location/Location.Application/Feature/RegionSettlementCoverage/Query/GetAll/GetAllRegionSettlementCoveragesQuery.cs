using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.RegionSettlementCoverage.Query.GetAll
{
    public sealed record GetAllRegionSettlementCoveragesQuery(GetRegionSettlementCoveragesRequest Request) : IQuery<Result<PaginatedResult<RegionSettlementCoverageResponse>>>;
}
