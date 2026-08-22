using BuildingBlocks.Messaging.Events;
using BuildingBlocks.Persistence;
using MassTransit;
using Microsoft.Extensions.Logging.Abstractions;
using PatientService.Application.Common.Interfaces;
using PatientService.Application.Common.Mappings;
using PatientService.Application.Common.Request;
using PatientService.Application.Common.Response;
using PatientService.Application.Feature.Patient.Command.Create;
using PatientService.Application.Feature.Patient.Command.Delete;
using PatientService.Application.Feature.Patient.Query.GetAll;
using PatientService.Application.Feature.Patient.Query.GetByKey;
using PatientService.Application.Feature.Patient.Query.GetCatalogs;
using System.Net;

namespace PatientService.UnitTests.Application.Handlers;

public class PatientHandlerTests
{
    private readonly IPatientWriteStore _writes = Substitute.For<IPatientWriteStore>();
    private readonly IPatientReadStore<PatientResponse> _reads = Substitute.For<IPatientReadStore<PatientResponse>>();
    private readonly IPatientReadStore<GenderResponse> _genders = Substitute.For<IPatientReadStore<GenderResponse>>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IFileStorageService _files = Substitute.For<IFileStorageService>();
    private readonly IPublishEndpoint _publishEndpoint = Substitute.For<IPublishEndpoint>();

    [Fact]
    public async Task Create_WhenGenderMissing_ReturnsNotFound()
    {
        _genders.GetByKeyAsync(Arg.Any<object>(), Arg.Any<CancellationToken>()).Returns((GenderResponse?)null);
        var handler = CreateHandler();

        var result = await handler.Handle(new CreatePatientCommand(ValidRequest()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await _writes.DidNotReceive().AddAsync(Arg.Any<Patient>(), Arg.Any<CancellationToken>());
        await _publishEndpoint.DidNotReceive().Publish(Arg.Any<CreateUserEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_WhenBloodTypeMissing_ReturnsNotFound()
    {
        ArrangeGender();
        _writes.FindAsync<BloodType>(Arg.Any<object[]>(), Arg.Any<CancellationToken>()).Returns((BloodType?)null);
        var handler = CreateHandler();
        var request = ValidRequest();
        request.BloodType = "XX";

        var result = await handler.Handle(new CreatePatientCommand(request), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await _writes.DidNotReceive().AddAsync(Arg.Any<Patient>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_WhenRequestIsValid_PersistsPatientAndPublishesUserEvent()
    {
        ArrangeGender();
        _writes.FindAsync<Language>(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(Language.Create("BS", "Bosnian"));
        _writes.FindAsync<Allergy>(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(Allergy.Create("IOD", "Iodine"));
        _writes.FindAsync<InsurancePayer>(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(InsurancePayer.Create("FBIH", "Federalni fond"));
        var handler = CreateHandler();
        var request = ValidRequest();
        request.AutoActivate = true;
        request.Allergies = "IOD";
        request.InsurancePayerCode = "FBIH";
        request.InsurancePolicyNumber = "POL-1";
        request.EmergencyContactName = "Aida Delić";
        request.EmergencyContactPhone = "+38762222444";
        request.EmergencyRelationshipCode = "SPOU";

        var result = await handler.Handle(new CreatePatientCommand(request), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Value.CurrentStatusCode.Should().Be(PatientStatusCodes.Active);
        result.Value.FirstName.Should().Be("Haris");
        await _writes.Received(1).AddAsync(Arg.Is<Patient>(patient =>
            patient.Email == "armanigas78@gmail.com"
            && patient.CurrentStatusCode == PatientStatusCodes.Active
            && patient.LanguageCoverages.Any(item => item.LanguageCode == "BS")
            && patient.AllergyCoverages.Any(item => item.AllergyCode == "IOD")
            && patient.CurrentInsurancePayerCode == "FBIH"
            && patient.EmergencyContacts.Count == 1
            && patient.ConsentCoverages.Count == 2), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await _publishEndpoint.Received(1).Publish(
            Arg.Is<CreateUserEvent>(evt => evt.Email == request.Email && evt.RoleName == "Patient"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_WhenNotAutoActivated_UsesPendingStatus()
    {
        ArrangeGender();
        _writes.FindAsync<Language>(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(Language.Create("BS", "Bosnian"));
        var handler = CreateHandler();
        var request = ValidRequest();
        request.AutoActivate = false;

        var result = await handler.Handle(new CreatePatientCommand(request), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.CurrentStatusCode.Should().Be(PatientStatusCodes.PendingVerification);
    }

    [Fact]
    public async Task Delete_WhenMissing_ReturnsNotFound()
    {
        _writes.FindAsync<Patient>(Arg.Any<object[]>(), Arg.Any<CancellationToken>()).Returns((Patient?)null);
        var handler = new DeletePatientCommandHandler(
            _writes,
            _unitOfWork,
            _files,
            _publishEndpoint,
            NullLogger<DeletePatientCommandHandler>.Instance);

        var result = await handler.Handle(new DeletePatientCommand(PatientFactory.DefaultId), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        _writes.DidNotReceive().Remove(Arg.Any<Patient>());
    }

    [Fact]
    public async Task Delete_WhenFound_RemovesEntityAndPicture()
    {
        var patient = PatientFactory.Create();
        _writes.FindAsync<Patient>(Arg.Any<object[]>(), Arg.Any<CancellationToken>()).Returns(patient);
        var handler = new DeletePatientCommandHandler(
            _writes,
            _unitOfWork,
            _files,
            _publishEndpoint,
            NullLogger<DeletePatientCommandHandler>.Instance);

        var result = await handler.Handle(new DeletePatientCommand(patient.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
        await _files.Received(1).DeleteFileAsync(patient.ProfilePictureUrl!);
        _writes.Received(1).Remove(patient);
        await _publishEndpoint.Received(1).Publish(Arg.Any<DeleteUserEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetByKey_WhenMissing_ReturnsNotFound()
    {
        _reads.GetByKeyAsync(Arg.Any<object>(), Arg.Any<CancellationToken>()).Returns((PatientResponse?)null);
        var handler = new GetPatientByKeyQueryHandler(_reads, NullLogger<GetPatientByKeyQueryHandler>.Instance);

        var result = await handler.Handle(new GetPatientByKeyQuery(PatientFactory.DefaultId), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetByKey_WhenFound_ReturnsResponse()
    {
        var response = PatientFactory.Create().ToResponse();
        _reads.GetByKeyAsync(Arg.Any<object>(), Arg.Any<CancellationToken>()).Returns(response);
        var handler = new GetPatientByKeyQueryHandler(_reads, NullLogger<GetPatientByKeyQueryHandler>.Instance);

        var result = await handler.Handle(new GetPatientByKeyQuery(response.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Email.Should().Be(response.Email);
    }

    [Fact]
    public async Task GetAll_ReturnsPagedResult()
    {
        var response = PatientFactory.Create().ToResponse();
        _reads.CountAsync(Arg.Any<object>(), Arg.Any<CancellationToken>()).Returns(1);
        _reads.GetPagedAsync(Arg.Any<object>(), Arg.Any<CancellationToken>())
            .Returns(new List<PatientResponse> { response });
        var handler = new GetAllPatientsQueryHandler(_reads, NullLogger<GetAllPatientsQueryHandler>.Instance);

        var result = await handler.Handle(
            new GetAllPatientsQuery(new GetPatientsRequest("haris")),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Count.Should().Be(1);
        result.Value.Data.Should().ContainSingle(item => item.Id == response.Id);
    }

    [Fact]
    public async Task GetCatalogs_AggregatesLookupLists()
    {
        var statuses = Substitute.For<IPatientReadStore<PatientStatusResponse>>();
        var genders = Substitute.For<IPatientReadStore<GenderResponse>>();
        var bloodTypes = Substitute.For<IPatientReadStore<BloodTypeResponse>>();
        var languages = Substitute.For<IPatientReadStore<LanguageResponse>>();
        var allergies = Substitute.For<IPatientReadStore<AllergyResponse>>();
        var conditions = Substitute.For<IPatientReadStore<ConditionResponse>>();
        var payers = Substitute.For<IPatientReadStore<InsurancePayerResponse>>();
        var relationships = Substitute.For<IPatientReadStore<RelationshipTypeResponse>>();
        var consents = Substitute.For<IPatientReadStore<ConsentTypeResponse>>();

        statuses.GetPagedAsync(Arg.Any<object>(), Arg.Any<CancellationToken>())
            .Returns(new List<PatientStatusResponse> { new() { Code = "ACT", Name = "Active" } });
        genders.GetPagedAsync(Arg.Any<object>(), Arg.Any<CancellationToken>())
            .Returns(new List<GenderResponse> { new() { Code = "M", Name = "Male" } });
        bloodTypes.GetPagedAsync(Arg.Any<object>(), Arg.Any<CancellationToken>()).Returns(new List<BloodTypeResponse>());
        languages.GetPagedAsync(Arg.Any<object>(), Arg.Any<CancellationToken>()).Returns(new List<LanguageResponse>());
        allergies.GetPagedAsync(Arg.Any<object>(), Arg.Any<CancellationToken>()).Returns(new List<AllergyResponse>());
        conditions.GetPagedAsync(Arg.Any<object>(), Arg.Any<CancellationToken>()).Returns(new List<ConditionResponse>());
        payers.GetPagedAsync(Arg.Any<object>(), Arg.Any<CancellationToken>()).Returns(new List<InsurancePayerResponse>());
        relationships.GetPagedAsync(Arg.Any<object>(), Arg.Any<CancellationToken>()).Returns(new List<RelationshipTypeResponse>());
        consents.GetPagedAsync(Arg.Any<object>(), Arg.Any<CancellationToken>()).Returns(new List<ConsentTypeResponse>());

        var handler = new GetPatientCatalogsQueryHandler(
            statuses, genders, bloodTypes, languages, allergies, conditions, payers, relationships, consents);

        var result = await handler.Handle(new GetPatientCatalogsQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Statuses.Should().ContainSingle(item => item.Code == "ACT");
        result.Value.Genders.Should().ContainSingle(item => item.Code == "M");
    }

    private CreatePatientCommandHandler CreateHandler() =>
        new(
            _writes,
            _genders,
            _unitOfWork,
            _files,
            _publishEndpoint,
            NullLogger<CreatePatientCommandHandler>.Instance);

    private void ArrangeGender()
    {
        _genders.GetByKeyAsync(Arg.Any<object>(), Arg.Any<CancellationToken>())
            .Returns(new GenderResponse { Code = "M", Name = "Male" });
    }

    private static CreatePatientRequest ValidRequest() =>
        new()
        {
            FirstName = "Haris",
            LastName = "Delić",
            Email = "armanigas78@gmail.com",
            PhoneNumber = "+38762222333",
            DateOfBirth = new DateTime(1975, 9, 3),
            Gender = "M",
            Languages = "BS"
        };
}
