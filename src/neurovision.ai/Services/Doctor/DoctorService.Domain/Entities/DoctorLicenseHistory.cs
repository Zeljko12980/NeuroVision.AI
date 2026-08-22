namespace DoctorService.Domain.Entities;

public class DoctorLicenseHistory
{
    public Guid DoctorId { get; private set; }
    public int SequenceNumber { get; private set; }
    public string LicenseNumber { get; private set; } = null!;
    public string? LicenseAuthorityCode { get; private set; }
    public DateTime From { get; private set; }
    public DateTime? To { get; private set; }

    public Doctor Doctor { get; private set; } = null!;
    public LicenseAuthority? LicenseAuthority { get; private set; }

    private DoctorLicenseHistory()
    {
    }

    public static DoctorLicenseHistory Create(
        Guid doctorId,
        int sequenceNumber,
        string licenseNumber,
        string? licenseAuthorityCode,
        DateTime from,
        DateTime? to = null)
    {
        if (doctorId == Guid.Empty)
            throw new ArgumentException("Doctor id is required.", nameof(doctorId));

        if (sequenceNumber <= 0)
            throw new ArgumentException("Sequence number must be greater than zero.", nameof(sequenceNumber));

        DateRange.EnsureValid(from, to);

        return new DoctorLicenseHistory
        {
            DoctorId = doctorId,
            SequenceNumber = sequenceNumber,
            LicenseNumber = Guard.NotEmpty(licenseNumber, nameof(licenseNumber)),
            LicenseAuthorityCode = string.IsNullOrWhiteSpace(licenseAuthorityCode)
                ? null
                : Guard.Code(licenseAuthorityCode, nameof(licenseAuthorityCode)),
            From = from,
            To = to
        };
    }

    public void Close(DateTime to)
    {
        DateRange.EnsureValid(From, to);
        To = to;
    }
}
