using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.GovernmentHistory.Query.GetByKey
{
    public sealed class GetGovernmentHistoryByKeyQueryHandler : IQueryHandler<GetGovernmentHistoryByKeyQuery, Result<GovernmentHistoryResponse>>
    {
        private readonly IGovernmentHistoryService _service;

        public GetGovernmentHistoryByKeyQueryHandler(IGovernmentHistoryService service)
        {
            _service = service;
        }

        public async Task<Result<GovernmentHistoryResponse>> Handle(GetGovernmentHistoryByKeyQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByKeyAsync(query.CountryCode, query.SequenceNumber, cancellationToken);
        }
    }
}
