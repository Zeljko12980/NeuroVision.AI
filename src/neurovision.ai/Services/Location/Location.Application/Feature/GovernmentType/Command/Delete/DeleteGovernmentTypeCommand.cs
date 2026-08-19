using BuildingBlocks.CQRS;
using BuildingBlocks.Results;

namespace LocationService.Application.Feature.GovernmentType.Command.Delete
{
    public sealed record DeleteGovernmentTypeCommand(string Code) : ICommand<Result<bool>>;
}
