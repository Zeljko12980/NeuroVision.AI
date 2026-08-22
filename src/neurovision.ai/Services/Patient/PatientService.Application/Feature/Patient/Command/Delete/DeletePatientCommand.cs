namespace PatientService.Application.Feature.Patient.Command.Delete;

public sealed record DeletePatientCommand(Guid Id) : ICommand<Result<bool>>;
