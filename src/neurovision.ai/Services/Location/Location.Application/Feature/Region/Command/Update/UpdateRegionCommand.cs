using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.Region.Command.Update
{
    public sealed record UpdateRegionCommand(UpdateRegionRequest Request, string TypeCode, short Code) : ICommand<Result<RegionResponse>>;
}
