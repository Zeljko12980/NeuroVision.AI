using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.Municipality.Query.GetByKey
{
    public sealed record GetMunicipalityByKeyQuery(string CountryCode, int Code) : IQuery<Result<MunicipalityResponse>>;
}
