using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.Country.Query.GetByCode
{
    public sealed record GetByCodeQuery(string Code) : IQuery<Result<CountryResponse>>;
   
}
