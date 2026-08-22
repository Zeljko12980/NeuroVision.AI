namespace DoctorService.Application.Feature.Doctor.Command.Delete;

public sealed record DeleteDoctorCommand(Guid Id) : ICommand<Result<bool>>;
