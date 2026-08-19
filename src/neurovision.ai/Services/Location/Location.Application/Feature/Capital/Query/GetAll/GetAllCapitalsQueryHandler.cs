using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.Capital.Query.GetAll
{
    public sealed class GetAllCapitalsQueryHandler : IQueryHandler<GetAllCapitalsQuery, Result<PaginatedResult<CapitalResponse>>>
    {
        private readonly ICapitalService _service;

        public GetAllCapitalsQueryHandler(ICapitalService service)
        {
            _service = service;
        }

        public async Task<Result<PaginatedResult<CapitalResponse>>> Handle(GetAllCapitalsQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync(query.Request, cancellationToken);
        }
    }
}
