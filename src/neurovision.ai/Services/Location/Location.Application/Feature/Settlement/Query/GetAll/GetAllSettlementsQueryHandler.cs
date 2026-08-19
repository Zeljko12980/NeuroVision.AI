using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.Settlement.Query.GetAll
{
    public sealed class GetAllSettlementsQueryHandler : IQueryHandler<GetAllSettlementsQuery, Result<PaginatedResult<SettlementResponse>>>
    {
        private readonly ISettlementService _service;

        public GetAllSettlementsQueryHandler(ISettlementService service)
        {
            _service = service;
        }

        public async Task<Result<PaginatedResult<SettlementResponse>>> Handle(GetAllSettlementsQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync(query.Request, cancellationToken);
        }
    }
}
