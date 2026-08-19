using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.MunicipalitySettlementCoverage.Query.GetByKey
{
    public sealed record GetMunicipalitySettlementCoverageByKeyQuery(string CountryCode, int MunicipalityCode, int SettlementCode) : IQuery<Result<MunicipalitySettlementCoverageResponse>>;
}
