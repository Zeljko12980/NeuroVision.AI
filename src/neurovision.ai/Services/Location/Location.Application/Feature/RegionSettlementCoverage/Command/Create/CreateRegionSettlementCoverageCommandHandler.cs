using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.RegionSettlementCoverage.Command.Create
{
    public sealed class CreateRegionSettlementCoverageCommandHandler : ICommandHandler<CreateRegionSettlementCoverageCommand, Result<RegionSettlementCoverageResponse>>
    {
        private readonly IRegionSettlementCoverageService _service;

        public CreateRegionSettlementCoverageCommandHandler(IRegionSettlementCoverageService service)
        {
            _service = service;
        }

        public async Task<Result<RegionSettlementCoverageResponse>> Handle(CreateRegionSettlementCoverageCommand command, CancellationToken cancellationToken)
        {
            return await _service.AddAsync(command.Request, cancellationToken);
        }
    }
}
