using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.LegalSuccessor.Query.GetAll
{
    public sealed class GetAllLegalSuccessorsQueryHandler : IQueryHandler<GetAllLegalSuccessorsQuery, Result<PaginatedResult<LegalSuccessorResponse>>>
    {
        private readonly ILegalSuccessorService _service;

        public GetAllLegalSuccessorsQueryHandler(ILegalSuccessorService service)
        {
            _service = service;
        }

        public async Task<Result<PaginatedResult<LegalSuccessorResponse>>> Handle(GetAllLegalSuccessorsQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync(query.Request, cancellationToken);
        }
    }
}
