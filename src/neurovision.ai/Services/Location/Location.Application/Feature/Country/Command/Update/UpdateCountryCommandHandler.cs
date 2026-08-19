using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.Country.Command.Update
{
    public sealed class UpdateCountryCommandHandler : ICommandHandler<UpdateCountryCommand, Result<CountryResponse>>
    {
        private readonly ICountryService _countryService;

        public UpdateCountryCommandHandler(ICountryService countryService)
        {
            _countryService = countryService;
        }
        public async Task<Result<CountryResponse>> Handle(UpdateCountryCommand command, CancellationToken cancellationToken)
        {
            return await _countryService.UpdateAsync(command.Code, command.Request, cancellationToken);
        }
    }
}
