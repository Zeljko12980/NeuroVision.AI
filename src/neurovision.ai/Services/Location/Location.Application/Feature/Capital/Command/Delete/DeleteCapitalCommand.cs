using BuildingBlocks.CQRS;
using BuildingBlocks.Results;

namespace LocationService.Application.Feature.Capital.Command.Delete
{
    public sealed record DeleteCapitalCommand(string CountryCode, int SettlementCode, int SequenceNumber) : ICommand<Result<bool>>;
}
