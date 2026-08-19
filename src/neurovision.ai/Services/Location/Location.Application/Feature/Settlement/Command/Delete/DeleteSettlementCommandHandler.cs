using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;

namespace LocationService.Application.Feature.Settlement.Command.Delete
{
    public sealed class DeleteSettlementCommandHandler : ICommandHandler<DeleteSettlementCommand, Result<bool>>
    {
        private readonly ISettlementService _service;

        public DeleteSettlementCommandHandler(ISettlementService service)
        {
            _service = service;
        }

        public async Task<Result<bool>> Handle(DeleteSettlementCommand command, CancellationToken cancellationToken)
        {
            return await _service.DeleteAsync(command.CountryCode, command.Code, cancellationToken);
        }
    }
}
