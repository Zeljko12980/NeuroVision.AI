using BuildingBlocks.CQRS;
using BuildingBlocks.Results;

namespace LocationService.Application.Feature.Settlement.Command.Delete
{
    public sealed record DeleteSettlementCommand(string CountryCode, int Code) : ICommand<Result<bool>>;
}
