using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.CountryComposition.Command.Update
{
    public sealed record UpdateCountryCompositionCommand(UpdateCountryCompositionRequest Request, string UnionCountryCode, string MemberCountryCode, int SequenceNumber) : ICommand<Result<CountryCompositionResponse>>;
}
