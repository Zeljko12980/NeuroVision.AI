using BuildingBlocks.Messaging.Events;
using BuildingBlocks.Persistence;
using DoctorService.Application.Common.Interfaces;
using DoctorService.Application.Common.Mappings;
using DoctorService.Application.Common.Request;
using DoctorService.Application.Common.Response;
using DoctorService.Application.Feature.Doctor.Command.Create;
using DoctorService.Application.Feature.Doctor.Command.Delete;
using DoctorService.Application.Feature.Doctor.Query.GetAll;
using DoctorService.Application.Feature.Doctor.Query.GetByKey;
using DoctorService.Application.Feature.Doctor.Query.GetCatalogs;
using MassTransit;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net;

namespace DoctorService.UnitTests.Application.Handlers;

public class DoctorHandlerTests
{
    private readonly IDoctorWriteStore _writes = Substitute.For<IDoctorWriteStore>();
    private readonly IDoctorReadStore<DoctorResponse> _reads = Substitute.For<IDoctorReadStore<DoctorResponse>>();
    private readonly IDoctorReadStore<SpecializationResponse> _specializations = Substitute.For<IDoctorReadStore<SpecializationResponse>>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IFileStorageService _files = Substitute.For<IFileStorageService>();
    private readonly IPublishEndpoint _publishEndpoint = Substitute.For<IPublishEndpoint>();

    [Fact]
    public async Task Create_WhenSpecializationMissing_ReturnsNotFound()
    {
        _specializations.GetByKeyAsync(Arg.Any<object>(), Arg.Any<CancellationToken>())
            .Returns((SpecializationResponse?)null);
        var handler = CreateHandler();

        var result = await handler.Handle(new CreateDoctorCommand(ValidRequest()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await _writes.DidNotReceive().AddAsync(Arg.Any<Doctor>(), Arg.Any<CancellationToken>());
        await _publishEndpoint.DidNotReceive().Publish(Arg.Any<CreateUserEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_WhenLicenseAuthorityMissing_ReturnsNotFound()
    {
        ArrangeSpecialization();
        _writes.FindAsync<LicenseAuthority>(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((LicenseAuthority?)null);
        var handler = CreateHandler();
        var request = ValidRequest();
        request.LicenseAuthorityCode = "XX";

        var result = await handler.Handle(new CreateDoctorCommand(request), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await _writes.DidNotReceive().AddAsync(Arg.Any<Doctor>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_WhenRequestIsValid_PersistsDoctorAndPublishesUserEvent()
    {
        ArrangeSpecialization();
        _writes.FindAsync<Language>(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(Language.Create("BS", "Bosnian"));
        _writes.FindAsync<DegreeType>(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(DegreeType.Create("MD", "Doctor of Medicine"));
        var handler = CreateHandler();
        var request = ValidRequest();
        request.AutoActivate = true;
        request.Degrees = "MD";
        request.Hospital = "Klinički centar Sarajevo";
        request.HealthInstitutionId = 1;
        request.IsAvailable = true;

        var result = await handler.Handle(new CreateDoctorCommand(request), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Value.CurrentStatusCode.Should().Be(DoctorStatusCodes.Active);
        result.Value.FirstName.Should().Be("Željko");
        await _writes.Received(1).AddAsync(Arg.Is<Doctor>(doctor =>
            doctor.Email == "ikanoviczeljko362@gmail.com"
            && doctor.CurrentStatusCode == DoctorStatusCodes.Active
            && doctor.CurrentSpecializationCode == "NEURO"
            && doctor.LanguageCoverages.Any(item => item.LanguageCode == "BS")
            && doctor.DegreeCoverages.Any(item => item.DegreeTypeCode == "MD")), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await _publishEndpoint.Received(1).Publish(
            Arg.Is<CreateUserEvent>(evt => evt.Email == request.Email && evt.RoleName == "Doctor"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_WhenNotAutoActivated_UsesPendingStatus()
    {
        ArrangeSpecialization();
        _writes.FindAsync<Language>(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(Language.Create("BS", "Bosnian"));
        var handler = CreateHandler();
        var request = ValidRequest();
        request.AutoActivate = false;

        var result = await handler.Handle(new CreateDoctorCommand(request), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.CurrentStatusCode.Should().Be(DoctorStatusCodes.PendingVerification);
    }

    [Fact]
    public async Task Delete_WhenMissing_ReturnsNotFound()
    {
        _writes.FindAsync<Doctor>(Arg.Any<object[]>(), Arg.Any<CancellationToken>()).Returns((Doctor?)null);
        var handler = new DeleteDoctorCommandHandler(
            _writes,
            _unitOfWork,
            _files,
            _publishEndpoint,
            NullLogger<DeleteDoctorCommandHandler>.Instance);

        var result = await handler.Handle(new DeleteDoctorCommand(DoctorFactory.DefaultId), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        _writes.DidNotReceive().Remove(Arg.Any<Doctor>());
    }

    [Fact]
    public async Task Delete_WhenFound_RemovesEntityAndPicture()
    {
        var doctor = DoctorFactory.Create();
        _writes.FindAsync<Doctor>(Arg.Any<object[]>(), Arg.Any<CancellationToken>()).Returns(doctor);
        var handler = new DeleteDoctorCommandHandler(
            _writes,
            _unitOfWork,
            _files,
            _publishEndpoint,
            NullLogger<DeleteDoctorCommandHandler>.Instance);

        var result = await handler.Handle(new DeleteDoctorCommand(doctor.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
        await _files.Received(1).DeleteFileAsync(doctor.ProfilePictureUrl!);
        _writes.Received(1).Remove(doctor);
        await _publishEndpoint.Received(1).Publish(Arg.Any<DeleteUserEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetByKey_WhenMissing_ReturnsNotFound()
    {
        _reads.GetByKeyAsync(Arg.Any<object>(), Arg.Any<CancellationToken>()).Returns((DoctorResponse?)null);
        var handler = new GetDoctorByKeyQueryHandler(_reads, NullLogger<GetDoctorByKeyQueryHandler>.Instance);

        var result = await handler.Handle(new GetDoctorByKeyQuery(DoctorFactory.DefaultId), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetByKey_WhenFound_ReturnsResponse()
    {
        var response = DoctorFactory.Create().ToResponse();
        _reads.GetByKeyAsync(Arg.Any<object>(), Arg.Any<CancellationToken>()).Returns(response);
        var handler = new GetDoctorByKeyQueryHandler(_reads, NullLogger<GetDoctorByKeyQueryHandler>.Instance);

        var result = await handler.Handle(new GetDoctorByKeyQuery(response.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Email.Should().Be(response.Email);
    }

    [Fact]
    public async Task GetAll_ReturnsPagedResult()
    {
        var response = DoctorFactory.Create().ToResponse();
        _reads.CountAsync(Arg.Any<object>(), Arg.Any<CancellationToken>()).Returns(1);
        _reads.GetPagedAsync(Arg.Any<object>(), Arg.Any<CancellationToken>())
            .Returns(new List<DoctorResponse> { response });
        var handler = new GetAllDoctorsQueryHandler(_reads, NullLogger<GetAllDoctorsQueryHandler>.Instance);

        var result = await handler.Handle(
            new GetAllDoctorsQuery(new GetDoctorsRequest("zeljko")),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Count.Should().Be(1);
        result.Value.Data.Should().ContainSingle(item => item.Id == response.Id);
    }

    [Fact]
    public async Task GetCatalogs_AggregatesLookupLists()
    {
        var specializations = Substitute.For<IDoctorReadStore<SpecializationResponse>>();
        var languages = Substitute.For<IDoctorReadStore<LanguageResponse>>();
        var degrees = Substitute.For<IDoctorReadStore<DegreeTypeResponse>>();
        var authorities = Substitute.For<IDoctorReadStore<LicenseAuthorityResponse>>();

        specializations.GetPagedAsync(Arg.Any<object>(), Arg.Any<CancellationToken>())
            .Returns(new List<SpecializationResponse> { new() { Code = "NEURO", Name = "Neurology" } });
        languages.GetPagedAsync(Arg.Any<object>(), Arg.Any<CancellationToken>())
            .Returns(new List<LanguageResponse> { new() { Code = "BS", Name = "Bosnian" } });
        degrees.GetPagedAsync(Arg.Any<object>(), Arg.Any<CancellationToken>()).Returns(new List<DegreeTypeResponse>());
        authorities.GetPagedAsync(Arg.Any<object>(), Arg.Any<CancellationToken>()).Returns(new List<LicenseAuthorityResponse>());

        var handler = new GetDoctorCatalogsQueryHandler(specializations, languages, degrees, authorities);

        var result = await handler.Handle(new GetDoctorCatalogsQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Specializations.Should().ContainSingle(item => item.Code == "NEURO");
        result.Value.Languages.Should().ContainSingle(item => item.Code == "BS");
    }

    private CreateDoctorCommandHandler CreateHandler() =>
        new(
            _writes,
            _specializations,
            _unitOfWork,
            _files,
            _publishEndpoint,
            NullLogger<CreateDoctorCommandHandler>.Instance);

    private void ArrangeSpecialization()
    {
        _specializations.GetByKeyAsync(Arg.Any<object>(), Arg.Any<CancellationToken>())
            .Returns(new SpecializationResponse { Code = "NEURO", Name = "Neurology" });
    }

    private static CreateDoctorRequest ValidRequest() =>
        new()
        {
            FirstName = "Željko",
            LastName = "Ikanović",
            LicenseNumber = "LIC-1001",
            Specialization = "NEURO",
            Email = "ikanoviczeljko362@gmail.com",
            PhoneNumber = "+38761111222",
            Languages = "BS"
        };
}
