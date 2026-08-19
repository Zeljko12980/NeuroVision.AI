using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.GovernmentType.Query.GetByKey
{
    public sealed class GetGovernmentTypeByKeyQueryHandler : IQueryHandler<GetGovernmentTypeByKeyQuery, Result<GovernmentTypeResponse>>
    {
        private readonly IGovernmentTypeService _service;

        public GetGovernmentTypeByKeyQueryHandler(IGovernmentTypeService service)
        {
            _service = service;
        }

        public async Task<Result<GovernmentTypeResponse>> Handle(GetGovernmentTypeByKeyQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByKeyAsync(query.Code, cancellationToken);
        }
    }
}
