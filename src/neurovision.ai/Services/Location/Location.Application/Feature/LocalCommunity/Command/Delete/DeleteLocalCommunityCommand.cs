using BuildingBlocks.CQRS;
using BuildingBlocks.Results;

namespace LocationService.Application.Feature.LocalCommunity.Command.Delete
{
    public sealed record DeleteLocalCommunityCommand(string CountryCode, int MunicipalityCode, int Identifier) : ICommand<Result<bool>>;
}
