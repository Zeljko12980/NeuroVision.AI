using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.Settlement.Command.Update
{
    public sealed class UpdateSettlementCommandHandler : ICommandHandler<UpdateSettlementCommand, Result<SettlementResponse>>
    {
        private readonly ISettlementService _service;

        public UpdateSettlementCommandHandler(ISettlementService service)
        {
            _service = service;
        }

        public async Task<Result<SettlementResponse>> Handle(UpdateSettlementCommand command, CancellationToken cancellationToken)
        {
            return await _service.UpdateAsync(command.CountryCode, command.Code, command.Request, cancellationToken);
        }
    }
}
