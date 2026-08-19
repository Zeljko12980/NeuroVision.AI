using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.RegionType.Command.Update
{
    public sealed record UpdateRegionTypeCommand(UpdateRegionTypeRequest Request, string Code) : ICommand<Result<RegionTypeResponse>>;
}
