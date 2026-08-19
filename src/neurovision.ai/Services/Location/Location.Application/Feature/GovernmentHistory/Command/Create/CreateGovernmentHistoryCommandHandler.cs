using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.GovernmentHistory.Command.Create
{
    public sealed class CreateGovernmentHistoryCommandHandler : ICommandHandler<CreateGovernmentHistoryCommand, Result<GovernmentHistoryResponse>>
    {
        private readonly IGovernmentHistoryService _service;

        public CreateGovernmentHistoryCommandHandler(IGovernmentHistoryService service)
        {
            _service = service;
        }

        public async Task<Result<GovernmentHistoryResponse>> Handle(CreateGovernmentHistoryCommand command, CancellationToken cancellationToken)
        {
            return await _service.AddAsync(command.Request, cancellationToken);
        }
    }
}
