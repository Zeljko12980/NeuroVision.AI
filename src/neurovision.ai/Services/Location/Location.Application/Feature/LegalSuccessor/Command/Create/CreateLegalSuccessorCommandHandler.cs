using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.LegalSuccessor.Command.Create
{
    public sealed class CreateLegalSuccessorCommandHandler : ICommandHandler<CreateLegalSuccessorCommand, Result<LegalSuccessorResponse>>
    {
        private readonly ILegalSuccessorService _service;

        public CreateLegalSuccessorCommandHandler(ILegalSuccessorService service)
        {
            _service = service;
        }

        public async Task<Result<LegalSuccessorResponse>> Handle(CreateLegalSuccessorCommand command, CancellationToken cancellationToken)
        {
            return await _service.AddAsync(command.Request, cancellationToken);
        }
    }
}
