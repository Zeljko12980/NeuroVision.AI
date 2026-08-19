using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;

namespace LocationService.Application.Feature.RegionSettlementCoverage.Command.Delete
{
    public sealed class DeleteRegionSettlementCoverageCommandHandler : ICommandHandler<DeleteRegionSettlementCoverageCommand, Result<bool>>
    {
        private readonly IRegionSettlementCoverageService _service;

        public DeleteRegionSettlementCoverageCommandHandler(IRegionSettlementCoverageService service)
        {
            _service = service;
        }

        public async Task<Result<bool>> Handle(DeleteRegionSettlementCoverageCommand command, CancellationToken cancellationToken)
        {
            return await _service.DeleteAsync(command.RegionTypeCode, command.RegionCode, command.CountryCode, command.SettlementCode, cancellationToken);
        }
    }
}
