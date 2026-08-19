using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.Country.Query.GetAll
{
    public sealed class GetAllCountriesQueryHandler : IQueryHandler<GetAllCountriesQuery, Result<PaginatedResult<CountryResponse>>>
    {
        private readonly ICountryService _countryService;

        public GetAllCountriesQueryHandler(ICountryService countryService)
        {
            _countryService = countryService;
        }
        public async Task<Result<PaginatedResult<CountryResponse>>> Handle(GetAllCountriesQuery query, CancellationToken cancellationToken)
        {
            return  await _countryService.GetAllAsync(query.Request, cancellationToken);
        }
    }
}
