using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.GovernmentHistory.Command.Update
{
    public sealed class UpdateGovernmentHistoryCommandHandler : ICommandHandler<UpdateGovernmentHistoryCommand, Result<GovernmentHistoryResponse>>
    {
        private readonly IGovernmentHistoryService _service;

        public UpdateGovernmentHistoryCommandHandler(IGovernmentHistoryService service)
        {
            _service = service;
        }

        public async Task<Result<GovernmentHistoryResponse>> Handle(UpdateGovernmentHistoryCommand command, CancellationToken cancellationToken)
        {
            return await _service.UpdateAsync(command.CountryCode, command.SequenceNumber, command.Request, cancellationToken);
        }
    }
}
