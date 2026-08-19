using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.GovernmentType.Command.Create
{
    public sealed record CreateGovernmentTypeCommand(CreateGovernmentTypeRequest Request) : ICommand<Result<GovernmentTypeResponse>>;
}
