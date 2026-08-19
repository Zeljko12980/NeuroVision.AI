using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.LocalCommunity.Command.Create
{
    public sealed record CreateLocalCommunityCommand(CreateLocalCommunityRequest Request) : ICommand<Result<LocalCommunityResponse>>;
}
