using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.HealthInstitutionType.Command.Create
{
    public sealed class CreateHealthInstitutionTypeCommandHandler : ICommandHandler<CreateHealthInstitutionTypeCommand, Result<HealthInstitutionTypeResponse>>
    {
        private readonly IHealthInstitutionTypeService _service;

        public CreateHealthInstitutionTypeCommandHandler(IHealthInstitutionTypeService service)
        {
            _service = service;
        }

        public async Task<Result<HealthInstitutionTypeResponse>> Handle(CreateHealthInstitutionTypeCommand command, CancellationToken cancellationToken)
        {
            return await _service.AddAsync(command.Request, cancellationToken);
        }
    }
}
