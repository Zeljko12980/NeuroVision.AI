using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.LocalCommunityCoverage.Command.Create
{
    public sealed class CreateLocalCommunityCoverageCommandHandler : ICommandHandler<CreateLocalCommunityCoverageCommand, Result<LocalCommunityCoverageResponse>>
    {
        private readonly ILocalCommunityCoverageService _service;

        public CreateLocalCommunityCoverageCommandHandler(ILocalCommunityCoverageService service)
        {
            _service = service;
        }

        public async Task<Result<LocalCommunityCoverageResponse>> Handle(CreateLocalCommunityCoverageCommand command, CancellationToken cancellationToken)
        {
            return await _service.AddAsync(command.Request, cancellationToken);
        }
    }
}
