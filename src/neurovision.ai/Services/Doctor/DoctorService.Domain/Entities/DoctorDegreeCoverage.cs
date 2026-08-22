namespace DoctorService.Domain.Entities;

public class DoctorDegreeCoverage
{
    public Guid DoctorId { get; private set; }
    public string DegreeTypeCode { get; private set; } = null!;
    public string? InstitutionName { get; private set; }
    public int? Year { get; private set; }

    public Doctor Doctor { get; private set; } = null!;
    public DegreeType DegreeType { get; private set; } = null!;

    private DoctorDegreeCoverage()
    {
    }

    public static DoctorDegreeCoverage Create(
        Guid doctorId,
        string degreeTypeCode,
        string? institutionName = null,
        int? year = null)
    {
        if (doctorId == Guid.Empty)
            throw new ArgumentException("Doctor id is required.", nameof(doctorId));

        if (year is < 1900 or > 2100)
            throw new ArgumentException("Year is out of range.", nameof(year));

        return new DoctorDegreeCoverage
        {
            DoctorId = doctorId,
            DegreeTypeCode = Guard.Code(degreeTypeCode, nameof(degreeTypeCode)),
            InstitutionName = string.IsNullOrWhiteSpace(institutionName) ? null : institutionName.Trim(),
            Year = year
        };
    }
}
