using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;

namespace LocationService.Application.Feature.MunicipalitySettlementCoverage.Command.Delete
{
    public sealed class DeleteMunicipalitySettlementCoverageCommandHandler : ICommandHandler<DeleteMunicipalitySettlementCoverageCommand, Result<bool>>
    {
        private readonly IMunicipalitySettlementCoverageService _service;

        public DeleteMunicipalitySettlementCoverageCommandHandler(IMunicipalitySettlementCoverageService service)
        {
            _service = service;
        }

        public async Task<Result<bool>> Handle(DeleteMunicipalitySettlementCoverageCommand command, CancellationToken cancellationToken)
        {
            return await _service.DeleteAsync(command.CountryCode, command.MunicipalityCode, command.SettlementCode, cancellationToken);
        }
    }
}
