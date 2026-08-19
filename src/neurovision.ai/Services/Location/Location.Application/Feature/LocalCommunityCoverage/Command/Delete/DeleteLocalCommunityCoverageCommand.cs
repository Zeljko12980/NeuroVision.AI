using BuildingBlocks.CQRS;
using BuildingBlocks.Results;

namespace LocationService.Application.Feature.LocalCommunityCoverage.Command.Delete
{
    public sealed record DeleteLocalCommunityCoverageCommand(string CountryCode, int MunicipalityCode, int LocalCommunityIdentifier, int SettlementCode) : ICommand<Result<bool>>;
}
