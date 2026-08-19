using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;

namespace LocationService.Application.Feature.GovernmentHistory.Command.Delete
{
    public sealed class DeleteGovernmentHistoryCommandHandler : ICommandHandler<DeleteGovernmentHistoryCommand, Result<bool>>
    {
        private readonly IGovernmentHistoryService _service;

        public DeleteGovernmentHistoryCommandHandler(IGovernmentHistoryService service)
        {
            _service = service;
        }

        public async Task<Result<bool>> Handle(DeleteGovernmentHistoryCommand command, CancellationToken cancellationToken)
        {
            return await _service.DeleteAsync(command.CountryCode, command.SequenceNumber, cancellationToken);
        }
    }
}
