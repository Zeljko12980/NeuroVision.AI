using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.LocalCommunityCoverage.Query.GetByKey
{
    public sealed record GetLocalCommunityCoverageByKeyQuery(string CountryCode, int MunicipalityCode, int LocalCommunityIdentifier, int SettlementCode) : IQuery<Result<LocalCommunityCoverageResponse>>;
}
