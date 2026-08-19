using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.LocalCommunity.Query.GetByKey
{
    public sealed record GetLocalCommunityByKeyQuery(string CountryCode, int MunicipalityCode, int Identifier) : IQuery<Result<LocalCommunityResponse>>;
}
