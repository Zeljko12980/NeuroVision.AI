namespace DoctorService.Application.Feature.WorkingSlot.Query.GetAll;

public sealed record GetAllWorkingSlotsQuery(GetWorkingSlotsRequest Request)
    : IQuery<Result<PaginatedResult<WorkingSlotResponse>>>;
