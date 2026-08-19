using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;

namespace LocationService.Application.Feature.Capital.Command.Delete
{
    public sealed class DeleteCapitalCommandHandler : ICommandHandler<DeleteCapitalCommand, Result<bool>>
    {
        private readonly ICapitalService _service;

        public DeleteCapitalCommandHandler(ICapitalService service)
        {
            _service = service;
        }

        public async Task<Result<bool>> Handle(DeleteCapitalCommand command, CancellationToken cancellationToken)
        {
            return await _service.DeleteAsync(command.CountryCode, command.SettlementCode, command.SequenceNumber, cancellationToken);
        }
    }
}
