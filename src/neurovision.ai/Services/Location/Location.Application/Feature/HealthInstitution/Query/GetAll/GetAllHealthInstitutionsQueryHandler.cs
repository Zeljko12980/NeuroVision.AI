using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.HealthInstitution.Query.GetAll
{
    public sealed class GetAllHealthInstitutionsQueryHandler : IQueryHandler<GetAllHealthInstitutionsQuery, Result<PaginatedResult<HealthInstitutionResponse>>>
    {
        private readonly IHealthInstitutionService _service;

        public GetAllHealthInstitutionsQueryHandler(IHealthInstitutionService service)
        {
            _service = service;
        }

        public async Task<Result<PaginatedResult<HealthInstitutionResponse>>> Handle(GetAllHealthInstitutionsQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync(query.Request, cancellationToken);
        }
    }
}
