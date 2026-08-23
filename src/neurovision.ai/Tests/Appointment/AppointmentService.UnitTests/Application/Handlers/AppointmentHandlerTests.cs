using BuildingBlocks.Persistence;
using MassTransit;
using Microsoft.Extensions.Logging.Abstractions;
using AppointmentService.Application.Common.Interfaces;
using AppointmentService.Application.Common.Response;
using AppointmentService.Application.Feature.Appointment.Command.Cancel;
using AppointmentService.Application.Feature.Appointment.Command.Create;
using AppointmentService.Application.Feature.Appointment.Command.Reschedule;
using AppointmentService.Application.Feature.Appointment.Query.GetById;
using AppointmentService.Application.Feature.Appointment.Query.GetRange;
using BuildingBlocks.Messaging.Events;
using System.Net;

namespace AppointmentService.UnitTests.Application.Handlers;

public class AppointmentHandlerTests
{
    private readonly IAppointmentWriteStore _writes = Substitute.For<IAppointmentWriteStore>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IPublishEndpoint _publishEndpoint = Substitute.For<IPublishEndpoint>();

    [Fact]
    public async Task Create_WhenTypeMissing_ReturnsNotFound()
    {
        _writes.TypeExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);
        var handler = CreateHandler();

        var result = await handler.Handle(ValidCreateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await _writes.DidNotReceive().AddAsync(Arg.Any<Appointment>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_WhenOverlap_ReturnsConflict()
    {
        _writes.TypeExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(true);
        _writes.HasOverlapAsync(
            Arg.Any<Guid>(),
            Arg.Any<DateTime>(),
            Arg.Any<DateTime>(),
            Arg.Any<Guid?>(),
            Arg.Any<CancellationToken>()).Returns(true);
        var handler = CreateHandler();

        var result = await handler.Handle(ValidCreateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Create_WhenValid_PersistsAndPublishesNotifications()
    {
        _writes.TypeExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(true);
        _writes.HasOverlapAsync(
            Arg.Any<Guid>(),
            Arg.Any<DateTime>(),
            Arg.Any<DateTime>(),
            Arg.Any<Guid?>(),
            Arg.Any<CancellationToken>()).Returns(false);
        var handler = CreateHandler();

        var result = await handler.Handle(ValidCreateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Value.PatientId.Should().Be(AppointmentFactory.PatientId);
        await _writes.Received(1).AddAsync(Arg.Any<Appointment>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await _publishEndpoint.Received(2).Publish(
            Arg.Is<CreateNotificationEvent>(item =>
                item.TypeCode == "APPT"
                && item.RelatedEntityType == "Appointment"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetById_WhenMissing_ReturnsNotFound()
    {
        _writes.FindAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Appointment?)null);
        var handler = new GetAppointmentByIdQueryHandler(
            _writes,
            NullLogger<GetAppointmentByIdQueryHandler>.Instance);

        var result = await handler.Handle(
            new GetAppointmentByIdQuery(AppointmentFactory.DefaultId, AppointmentFactory.PatientActor),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetRange_WhenInvalidInterval_ReturnsBadRequest()
    {
        var handler = new GetAppointmentRangeQueryHandler(
            _writes,
            NullLogger<GetAppointmentRangeQueryHandler>.Instance);

        var result = await handler.Handle(
            new GetAppointmentRangeQuery(
                AppointmentFactory.EndsAt,
                AppointmentFactory.StartsAt,
                AppointmentFactory.DoctorActor,
                null,
                null),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetRange_WhenValid_ReturnsItems()
    {
        var appointment = AppointmentFactory.Create();
        _writes.GetRangeAsync(
            Arg.Any<DateTime>(),
            Arg.Any<DateTime>(),
            Arg.Any<Guid?>(),
            Arg.Any<Guid?>(),
            Arg.Any<CancellationToken>()).Returns(new List<Appointment> { appointment });
        var handler = new GetAppointmentRangeQueryHandler(
            _writes,
            NullLogger<GetAppointmentRangeQueryHandler>.Instance);

        var result = await handler.Handle(
            new GetAppointmentRangeQuery(
                AppointmentFactory.StartsAt.AddHours(-1),
                AppointmentFactory.EndsAt.AddHours(1),
                AppointmentFactory.DoctorActor,
                AppointmentFactory.PatientId,
                AppointmentFactory.DoctorId),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().ContainSingle(item => item.Id == appointment.Id);
    }

    [Fact]
    public async Task Reschedule_WhenOverlap_ReturnsConflict()
    {
        var appointment = AppointmentFactory.Create();
        _writes.FindAsync(appointment.Id, Arg.Any<CancellationToken>()).Returns(appointment);
        _writes.HasOverlapAsync(
            appointment.DoctorId,
            Arg.Any<DateTime>(),
            Arg.Any<DateTime>(),
            appointment.Id,
            Arg.Any<CancellationToken>()).Returns(true);
        var handler = new RescheduleAppointmentCommandHandler(
            _writes,
            _unitOfWork,
            _publishEndpoint,
            NullLogger<RescheduleAppointmentCommandHandler>.Instance);

        var result = await handler.Handle(
            new RescheduleAppointmentCommand(
                appointment.Id,
                new RescheduleAppointmentRequest
                {
                    StartsAt = AppointmentFactory.StartsAt.AddHours(2),
                    EndsAt = AppointmentFactory.EndsAt.AddHours(2),
                    Title = "Moved"
                },
                AppointmentFactory.DoctorActor),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Cancel_WhenFound_SetsCancelledAndPublishes()
    {
        var appointment = AppointmentFactory.Create();
        _writes.FindAsync(appointment.Id, Arg.Any<CancellationToken>()).Returns(appointment);
        var handler = new CancelAppointmentCommandHandler(
            _writes,
            _unitOfWork,
            _publishEndpoint,
            NullLogger<CancelAppointmentCommandHandler>.Instance);

        var result = await handler.Handle(
            new CancelAppointmentCommand(appointment.Id, AppointmentFactory.DoctorActor),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.StatusCode.Should().Be(AppointmentStatusCodes.Cancelled);
        appointment.CancelledAt.Should().NotBeNull();
        await _publishEndpoint.Received(2).Publish(
            Arg.Any<CreateNotificationEvent>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_WhenPatientUsesAnotherPatientId_ReturnsForbidden()
    {
        var handler = CreateHandler();
        var command = new CreateAppointmentCommand(
            new CreateAppointmentRequest
            {
                PatientId = AppointmentFactory.PatientId,
                DoctorId = AppointmentFactory.DoctorId,
                TypeCode = AppointmentTypeCodes.Consultation,
                StartsAt = AppointmentFactory.StartsAt,
                EndsAt = AppointmentFactory.EndsAt,
                Title = "Consultation"
            },
            AppointmentFactory.OtherPatientActor);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        await _writes.DidNotReceive().AddAsync(Arg.Any<Appointment>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetById_WhenOtherPatient_ReturnsNotFound()
    {
        var appointment = AppointmentFactory.Create();
        _writes.FindAsync(appointment.Id, Arg.Any<CancellationToken>()).Returns(appointment);
        var handler = new GetAppointmentByIdQueryHandler(
            _writes,
            NullLogger<GetAppointmentByIdQueryHandler>.Instance);

        var result = await handler.Handle(
            new GetAppointmentByIdQuery(appointment.Id, AppointmentFactory.OtherPatientActor),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetRange_WhenPatient_ScopesToOwnId()
    {
        _writes.GetRangeAsync(
            Arg.Any<DateTime>(),
            Arg.Any<DateTime>(),
            AppointmentFactory.PatientId,
            null,
            Arg.Any<CancellationToken>()).Returns(new List<Appointment>());
        var handler = new GetAppointmentRangeQueryHandler(
            _writes,
            NullLogger<GetAppointmentRangeQueryHandler>.Instance);

        var result = await handler.Handle(
            new GetAppointmentRangeQuery(
                AppointmentFactory.StartsAt.AddHours(-1),
                AppointmentFactory.EndsAt.AddHours(1),
                AppointmentFactory.PatientActor,
                Guid.NewGuid(),
                AppointmentFactory.DoctorId),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _writes.Received(1).GetRangeAsync(
            Arg.Any<DateTime>(),
            Arg.Any<DateTime>(),
            AppointmentFactory.PatientId,
            null,
            Arg.Any<CancellationToken>());
    }

    private CreateAppointmentCommandHandler CreateHandler() =>
        new(
            _writes,
            _unitOfWork,
            _publishEndpoint,
            NullLogger<CreateAppointmentCommandHandler>.Instance);

    private static CreateAppointmentCommand ValidCreateCommand() =>
        new(new CreateAppointmentRequest
        {
            PatientId = AppointmentFactory.PatientId,
            DoctorId = AppointmentFactory.DoctorId,
            TypeCode = AppointmentTypeCodes.Consultation,
            StartsAt = AppointmentFactory.StartsAt,
            EndsAt = AppointmentFactory.EndsAt,
            Title = "Consultation"
        }, AppointmentFactory.DoctorActor);
}
