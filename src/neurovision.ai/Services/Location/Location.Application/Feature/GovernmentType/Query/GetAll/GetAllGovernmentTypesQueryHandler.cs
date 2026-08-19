using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.GovernmentType.Query.GetAll
{
    public sealed class GetAllGovernmentTypesQueryHandler : IQueryHandler<GetAllGovernmentTypesQuery, Result<PaginatedResult<GovernmentTypeResponse>>>
    {
        private readonly IGovernmentTypeService _service;

        public GetAllGovernmentTypesQueryHandler(IGovernmentTypeService service)
        {
            _service = service;
        }

        public async Task<Result<PaginatedResult<GovernmentTypeResponse>>> Handle(GetAllGovernmentTypesQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync(query.Request, cancellationToken);
        }
    }
}
