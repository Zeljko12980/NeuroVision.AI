using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.GovernmentType.Command.Create
{
    public sealed class CreateGovernmentTypeCommandHandler : ICommandHandler<CreateGovernmentTypeCommand, Result<GovernmentTypeResponse>>
    {
        private readonly IGovernmentTypeService _service;

        public CreateGovernmentTypeCommandHandler(IGovernmentTypeService service)
        {
            _service = service;
        }

        public async Task<Result<GovernmentTypeResponse>> Handle(CreateGovernmentTypeCommand command, CancellationToken cancellationToken)
        {
            return await _service.AddAsync(command.Request, cancellationToken);
        }
    }
}
