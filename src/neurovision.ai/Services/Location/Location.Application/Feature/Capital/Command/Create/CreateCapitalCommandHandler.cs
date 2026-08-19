using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.Capital.Command.Create
{
    public sealed class CreateCapitalCommandHandler : ICommandHandler<CreateCapitalCommand, Result<CapitalResponse>>
    {
        private readonly ICapitalService _service;

        public CreateCapitalCommandHandler(ICapitalService service)
        {
            _service = service;
        }

        public async Task<Result<CapitalResponse>> Handle(CreateCapitalCommand command, CancellationToken cancellationToken)
        {
            return await _service.AddAsync(command.Request, cancellationToken);
        }
    }
}
