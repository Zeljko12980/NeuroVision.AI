using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.Settlement.Command.Create
{
    public sealed record CreateSettlementCommand(CreateSettlementRequest Request) : ICommand<Result<SettlementResponse>>;
}
