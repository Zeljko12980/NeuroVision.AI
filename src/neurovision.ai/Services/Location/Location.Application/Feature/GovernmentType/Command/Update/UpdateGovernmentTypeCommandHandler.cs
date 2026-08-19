using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.GovernmentType.Command.Update
{
    public sealed class UpdateGovernmentTypeCommandHandler : ICommandHandler<UpdateGovernmentTypeCommand, Result<GovernmentTypeResponse>>
    {
        private readonly IGovernmentTypeService _service;

        public UpdateGovernmentTypeCommandHandler(IGovernmentTypeService service)
        {
            _service = service;
        }

        public async Task<Result<GovernmentTypeResponse>> Handle(UpdateGovernmentTypeCommand command, CancellationToken cancellationToken)
        {
            return await _service.UpdateAsync(command.Code, command.Request, cancellationToken);
        }
    }
}
