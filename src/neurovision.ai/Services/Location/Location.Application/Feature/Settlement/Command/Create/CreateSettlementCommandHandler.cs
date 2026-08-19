using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.Settlement.Command.Create
{
    public sealed class CreateSettlementCommandHandler : ICommandHandler<CreateSettlementCommand, Result<SettlementResponse>>
    {
        private readonly ISettlementService _service;

        public CreateSettlementCommandHandler(ISettlementService service)
        {
            _service = service;
        }

        public async Task<Result<SettlementResponse>> Handle(CreateSettlementCommand command, CancellationToken cancellationToken)
        {
            return await _service.AddAsync(command.Request, cancellationToken);
        }
    }
}
