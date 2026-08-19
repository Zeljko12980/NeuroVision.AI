using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.CountryComposition.Query.GetAll
{
    public sealed class GetAllCountryCompositionsQueryHandler : IQueryHandler<GetAllCountryCompositionsQuery, Result<PaginatedResult<CountryCompositionResponse>>>
    {
        private readonly ICountryCompositionService _service;

        public GetAllCountryCompositionsQueryHandler(ICountryCompositionService service)
        {
            _service = service;
        }

        public async Task<Result<PaginatedResult<CountryCompositionResponse>>> Handle(GetAllCountryCompositionsQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync(query.Request, cancellationToken);
        }
    }
}
