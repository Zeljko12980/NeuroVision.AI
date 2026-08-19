using BuildingBlocks.CQRS;
using BuildingBlocks.Results;

namespace LocationService.Application.Feature.RegionType.Command.Delete
{
    public sealed record DeleteRegionTypeCommand(string Code) : ICommand<Result<bool>>;
}
