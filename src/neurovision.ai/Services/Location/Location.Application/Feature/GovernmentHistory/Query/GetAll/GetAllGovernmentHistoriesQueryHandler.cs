using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.GovernmentHistory.Query.GetAll
{
    public sealed class GetAllGovernmentHistoriesQueryHandler : IQueryHandler<GetAllGovernmentHistoriesQuery, Result<PaginatedResult<GovernmentHistoryResponse>>>
    {
        private readonly IGovernmentHistoryService _service;

        public GetAllGovernmentHistoriesQueryHandler(IGovernmentHistoryService service)
        {
            _service = service;
        }

        public async Task<Result<PaginatedResult<GovernmentHistoryResponse>>> Handle(GetAllGovernmentHistoriesQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync(query.Request, cancellationToken);
        }
    }
}
