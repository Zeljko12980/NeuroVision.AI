using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.CountryComposition.Query.GetByKey
{
    public sealed record GetCountryCompositionByKeyQuery(string UnionCountryCode, string MemberCountryCode, int SequenceNumber) : IQuery<Result<CountryCompositionResponse>>;
}
