using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.RegionType.Command.Create
{
    public sealed record CreateRegionTypeCommand(CreateRegionTypeRequest Request) : ICommand<Result<RegionTypeResponse>>;
}
