using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.RegionSettlementCoverage.Query.GetAll
{
    public sealed class GetAllRegionSettlementCoveragesQueryHandler : IQueryHandler<GetAllRegionSettlementCoveragesQuery, Result<PaginatedResult<RegionSettlementCoverageResponse>>>
    {
        private readonly IRegionSettlementCoverageService _service;

        public GetAllRegionSettlementCoveragesQueryHandler(IRegionSettlementCoverageService service)
        {
            _service = service;
        }

        public async Task<Result<PaginatedResult<RegionSettlementCoverageResponse>>> Handle(GetAllRegionSettlementCoveragesQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync(query.Request, cancellationToken);
        }
    }
}
