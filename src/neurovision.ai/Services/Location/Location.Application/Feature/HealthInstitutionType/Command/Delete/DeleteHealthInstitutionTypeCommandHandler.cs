using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;

namespace LocationService.Application.Feature.HealthInstitutionType.Command.Delete
{
    public sealed class DeleteHealthInstitutionTypeCommandHandler : ICommandHandler<DeleteHealthInstitutionTypeCommand, Result<bool>>
    {
        private readonly IHealthInstitutionTypeService _service;

        public DeleteHealthInstitutionTypeCommandHandler(IHealthInstitutionTypeService service)
        {
            _service = service;
        }

        public async Task<Result<bool>> Handle(DeleteHealthInstitutionTypeCommand command, CancellationToken cancellationToken)
        {
            return await _service.DeleteAsync(command.Code, cancellationToken);
        }
    }
}
