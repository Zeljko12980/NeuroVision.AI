using BuildingBlocks.CQRS;
using BuildingBlocks.Results;

namespace LocationService.Application.Feature.MunicipalitySettlementCoverage.Command.Delete
{
    public sealed record DeleteMunicipalitySettlementCoverageCommand(string CountryCode, int MunicipalityCode, int SettlementCode) : ICommand<Result<bool>>;
}
