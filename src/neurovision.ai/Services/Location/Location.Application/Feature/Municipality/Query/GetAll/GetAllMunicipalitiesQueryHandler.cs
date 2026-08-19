using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.Municipality.Query.GetAll
{
    public sealed class GetAllMunicipalitiesQueryHandler : IQueryHandler<GetAllMunicipalitiesQuery, Result<PaginatedResult<MunicipalityResponse>>>
    {
        private readonly IMunicipalityService _service;

        public GetAllMunicipalitiesQueryHandler(IMunicipalityService service)
        {
            _service = service;
        }

        public async Task<Result<PaginatedResult<MunicipalityResponse>>> Handle(GetAllMunicipalitiesQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync(query.Request, cancellationToken);
        }
    }
}
