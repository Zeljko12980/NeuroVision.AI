using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.GovernmentHistory.Command.Create
{
    public sealed record CreateGovernmentHistoryCommand(CreateGovernmentHistoryRequest Request) : ICommand<Result<GovernmentHistoryResponse>>;
}
