using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.CountryComposition.Query.GetAll
{
    public sealed record GetAllCountryCompositionsQuery(GetCountryCompositionsRequest Request) : IQuery<Result<PaginatedResult<CountryCompositionResponse>>>;
}
