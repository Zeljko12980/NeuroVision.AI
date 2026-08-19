using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.RegionSettlementCoverage.Query.GetByKey
{
    public sealed class GetRegionSettlementCoverageByKeyQueryHandler : IQueryHandler<GetRegionSettlementCoverageByKeyQuery, Result<RegionSettlementCoverageResponse>>
    {
        private readonly IRegionSettlementCoverageService _service;

        public GetRegionSettlementCoverageByKeyQueryHandler(IRegionSettlementCoverageService service)
        {
            _service = service;
        }

        public async Task<Result<RegionSettlementCoverageResponse>> Handle(GetRegionSettlementCoverageByKeyQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByKeyAsync(query.RegionTypeCode, query.RegionCode, query.CountryCode, query.SettlementCode, cancellationToken);
        }
    }
}
