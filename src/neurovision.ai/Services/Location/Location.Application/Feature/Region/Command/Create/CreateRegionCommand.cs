using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.Region.Command.Create
{
    public sealed record CreateRegionCommand(CreateRegionRequest Request) : ICommand<Result<RegionResponse>>;
}
