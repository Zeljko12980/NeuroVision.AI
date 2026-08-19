using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.Capital.Command.Update
{
    public sealed class UpdateCapitalCommandHandler : ICommandHandler<UpdateCapitalCommand, Result<CapitalResponse>>
    {
        private readonly ICapitalService _service;

        public UpdateCapitalCommandHandler(ICapitalService service)
        {
            _service = service;
        }

        public async Task<Result<CapitalResponse>> Handle(UpdateCapitalCommand command, CancellationToken cancellationToken)
        {
            return await _service.UpdateAsync(command.CountryCode, command.SettlementCode, command.SequenceNumber, command.Request, cancellationToken);
        }
    }
}
