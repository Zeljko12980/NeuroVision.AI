namespace AppointmentService.UnitTests.Domain;

public class AppointmentTests
{
    [Fact]
    public void Create_WithValidData_SetsScheduledFields()
    {
        var appointment = AppointmentFactory.Create();

        appointment.PatientId.Should().Be(AppointmentFactory.PatientId);
        appointment.DoctorId.Should().Be(AppointmentFactory.DoctorId);
        appointment.TypeCode.Should().Be(AppointmentTypeCodes.Consultation);
        appointment.StatusCode.Should().Be(AppointmentStatusCodes.Scheduled);
        appointment.Title.Should().Be("Consultation");
        appointment.CancelledAt.Should().BeNull();
    }

    [Fact]
    public void Create_WithEmptyId_Throws()
    {
        var act = () => AppointmentFactory.Create(id: Guid.Empty);

        act.Should().Throw<ArgumentException>().WithParameterName("id");
    }

    [Fact]
    public void Create_WithEmptyPatient_Throws()
    {
        var act = () => AppointmentFactory.Create(patientId: Guid.Empty);

        act.Should().Throw<ArgumentException>().WithParameterName("patientId");
    }

    [Fact]
    public void Create_WithEmptyDoctor_Throws()
    {
        var act = () => AppointmentFactory.Create(doctorId: Guid.Empty);

        act.Should().Throw<ArgumentException>().WithParameterName("doctorId");
    }

    [Fact]
    public void Create_WhenEndIsNotAfterStart_Throws()
    {
        var act = () => AppointmentFactory.Create(
            startsAt: AppointmentFactory.StartsAt,
            endsAt: AppointmentFactory.StartsAt);

        act.Should().Throw<ArgumentException>().WithParameterName("endsAt");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidTitle_Throws(string? title)
    {
        var act = () => AppointmentFactory.Create(title: title!);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Overlaps_WhenIntervalsIntersect_ReturnsTrue()
    {
        var appointment = AppointmentFactory.Create();

        appointment.Overlaps(
            AppointmentFactory.StartsAt.AddMinutes(15),
            AppointmentFactory.EndsAt.AddMinutes(15)).Should().BeTrue();
    }

    [Fact]
    public void Overlaps_WhenIntervalsTouch_ReturnsFalse()
    {
        var appointment = AppointmentFactory.Create();

        appointment.Overlaps(AppointmentFactory.EndsAt, AppointmentFactory.EndsAt.AddMinutes(30))
            .Should().BeFalse();
    }

    [Fact]
    public void Cancel_SetsCancelledStatusOnce()
    {
        var appointment = AppointmentFactory.Create();
        var cancelledAt = AppointmentFactory.CreatedAt.AddMinutes(5);

        appointment.Cancel(cancelledAt);

        appointment.StatusCode.Should().Be(AppointmentStatusCodes.Cancelled);
        appointment.CancelledAt.Should().Be(cancelledAt);
        var act = () => appointment.Cancel(cancelledAt.AddMinutes(5));
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Reschedule_WhenCancelled_Throws()
    {
        var appointment = AppointmentFactory.Create();
        appointment.Cancel(AppointmentFactory.CreatedAt);

        var act = () => appointment.Reschedule(
            AppointmentFactory.StartsAt.AddHours(1),
            AppointmentFactory.EndsAt.AddHours(1),
            "Moved",
            null);

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Complete_SetsCompletedStatus()
    {
        var appointment = AppointmentFactory.Create();
        var completedAt = AppointmentFactory.EndsAt;

        appointment.Complete(completedAt);

        appointment.StatusCode.Should().Be(AppointmentStatusCodes.Completed);
        appointment.CompletedAt.Should().Be(completedAt);
    }
}
