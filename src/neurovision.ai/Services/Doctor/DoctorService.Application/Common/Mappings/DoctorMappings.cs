namespace DoctorService.Application.Common.Mappings;

public static class DoctorMappings
{
    public static DoctorResponse ToResponse(this Doctor entity) =>
        new()
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email,
            Phone = entity.Phone,
            LicenseNumber = entity.LicenseNumber,
            LicenseAuthorityCode = entity.LicenseAuthorityCode,
            CurrentSpecializationCode = entity.CurrentSpecializationCode,
            CurrentStatusCode = entity.CurrentStatusCode,
            ProfilePictureUrl = entity.ProfilePictureUrl,
            Bio = entity.Bio,
            CurrentHealthInstitutionId = entity.CurrentHealthInstitutionId,
            CurrentInstitutionName = entity.CurrentInstitutionName,
            IsAvailable = entity.IsAvailable,
            LastActive = entity.LastActive,
            AverageRating = entity.AverageRating,
            TotalReviews = entity.TotalReviews,
            CreatedAt = entity.CreatedAt
        };

    public static SpecializationResponse ToResponse(this Specialization entity) =>
        new()
        {
            Code = entity.Code,
            Name = entity.Name,
            Description = entity.Description
        };
}
