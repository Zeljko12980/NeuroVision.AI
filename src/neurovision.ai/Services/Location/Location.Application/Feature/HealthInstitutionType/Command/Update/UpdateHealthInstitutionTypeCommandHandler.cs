using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.HealthInstitutionType.Command.Update
{
    public sealed class UpdateHealthInstitutionTypeCommandHandler : ICommandHandler<UpdateHealthInstitutionTypeCommand, Result<HealthInstitutionTypeResponse>>
    {
        private readonly IHealthInstitutionTypeService _service;

        public UpdateHealthInstitutionTypeCommandHandler(IHealthInstitutionTypeService service)
        {
            _service = service;
        }

        public async Task<Result<HealthInstitutionTypeResponse>> Handle(UpdateHealthInstitutionTypeCommand command, CancellationToken cancellationToken)
        {
            return await _service.UpdateAsync(command.Code, command.Request, cancellationToken);
        }
    }
}
