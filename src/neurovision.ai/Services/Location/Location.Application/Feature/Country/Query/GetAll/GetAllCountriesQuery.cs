using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.Country.Query.GetAll
{
    public sealed record GetAllCountriesQuery(GetCountriesRequest Request) : IQuery<Result<PaginatedResult<CountryResponse>>>;
}
