namespace PatientService.Application.Common.Mappings;

public static class PatientMappings
{
    public static PatientResponse ToResponse(this Patient entity) =>
        new()
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email,
            Phone = entity.Phone,
            DateOfBirth = DateOnly.FromDateTime(entity.DateOfBirth),
            GenderCode = entity.GenderCode,
            BloodTypeCode = entity.BloodTypeCode,
            NationalId = entity.NationalId,
            CurrentStatusCode = entity.CurrentStatusCode,
            ProfilePictureUrl = entity.ProfilePictureUrl,
            Notes = entity.Notes,
            CurrentHealthInstitutionId = entity.CurrentHealthInstitutionId,
            CurrentInstitutionName = entity.CurrentInstitutionName,
            AssignedDoctorId = entity.AssignedDoctorId,
            CurrentInsurancePayerCode = entity.CurrentInsurancePayerCode,
            CurrentInsurancePolicyNumber = entity.CurrentInsurancePolicyNumber,
            AddressLine = entity.AddressLine,
            SettlementId = entity.SettlementId,
            MunicipalityId = entity.MunicipalityId,
            CountryId = entity.CountryId,
            HeightCm = entity.HeightCm,
            WeightKg = entity.WeightKg,
            LastActive = entity.LastActive,
            CreatedAt = entity.CreatedAt
        };
}
