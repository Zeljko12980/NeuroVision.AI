using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.RegionSettlementCoverage.Query.GetByKey
{
    public sealed record GetRegionSettlementCoverageByKeyQuery(string RegionTypeCode, short RegionCode, string CountryCode, int SettlementCode) : IQuery<Result<RegionSettlementCoverageResponse>>;
}
