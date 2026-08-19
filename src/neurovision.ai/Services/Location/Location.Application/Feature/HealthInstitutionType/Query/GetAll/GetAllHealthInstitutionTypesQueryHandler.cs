using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.HealthInstitutionType.Query.GetAll
{
    public sealed class GetAllHealthInstitutionTypesQueryHandler : IQueryHandler<GetAllHealthInstitutionTypesQuery, Result<PaginatedResult<HealthInstitutionTypeResponse>>>
    {
        private readonly IHealthInstitutionTypeService _service;

        public GetAllHealthInstitutionTypesQueryHandler(IHealthInstitutionTypeService service)
        {
            _service = service;
        }

        public async Task<Result<PaginatedResult<HealthInstitutionTypeResponse>>> Handle(GetAllHealthInstitutionTypesQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync(query.Request, cancellationToken);
        }
    }
}
