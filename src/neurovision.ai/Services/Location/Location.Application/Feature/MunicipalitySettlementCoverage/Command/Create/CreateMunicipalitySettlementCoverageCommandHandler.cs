using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.MunicipalitySettlementCoverage.Command.Create
{
    public sealed class CreateMunicipalitySettlementCoverageCommandHandler : ICommandHandler<CreateMunicipalitySettlementCoverageCommand, Result<MunicipalitySettlementCoverageResponse>>
    {
        private readonly IMunicipalitySettlementCoverageService _service;

        public CreateMunicipalitySettlementCoverageCommandHandler(IMunicipalitySettlementCoverageService service)
        {
            _service = service;
        }

        public async Task<Result<MunicipalitySettlementCoverageResponse>> Handle(CreateMunicipalitySettlementCoverageCommand command, CancellationToken cancellationToken)
        {
            return await _service.AddAsync(command.Request, cancellationToken);
        }
    }
}
