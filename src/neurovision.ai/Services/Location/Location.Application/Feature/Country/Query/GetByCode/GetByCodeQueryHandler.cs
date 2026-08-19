using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.Country.Query.GetByCode
{
    public sealed class GetByCodeQueryHandler:IQueryHandler<GetByCodeQuery, Result<CountryResponse>>
    {
        private readonly ICountryService _countryService;
        public GetByCodeQueryHandler(ICountryService countryService)
        {
            _countryService = countryService;
        }
        public async Task<Result<CountryResponse>> Handle(GetByCodeQuery query, CancellationToken cancellationToken)
        {
            return await _countryService.GetByCodeAsync(query.Code, cancellationToken);
        }
    }
}
