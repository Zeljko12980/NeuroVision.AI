using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;

namespace LocationService.Application.Feature.Country.Command.Delete
{
    public sealed class DeleteCountryCommandHandler : ICommandHandler<DeleteCountryCommand, Result<bool>>
    {
        private readonly ICountryService _countryService;

        public DeleteCountryCommandHandler(ICountryService countryService)
        {
            _countryService = countryService;
        }
    
        public async Task<Result<bool>> Handle(DeleteCountryCommand command, CancellationToken cancellationToken)
        {
            return await _countryService.DeleteAsync(command.Code, cancellationToken);
        }
    }
}
