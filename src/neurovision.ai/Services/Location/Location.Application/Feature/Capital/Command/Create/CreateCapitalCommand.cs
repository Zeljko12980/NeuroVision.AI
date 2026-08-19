using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.Capital.Command.Create
{
    public sealed record CreateCapitalCommand(CreateCapitalRequest Request) : ICommand<Result<CapitalResponse>>;
}
