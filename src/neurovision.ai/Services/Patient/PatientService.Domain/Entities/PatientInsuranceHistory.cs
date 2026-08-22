namespace PatientService.Domain.Entities;

public class PatientInsuranceHistory
{
    public Guid PatientId { get; private set; }
    public int SequenceNumber { get; private set; }
    public string PayerCode { get; private set; } = null!;
    public string PolicyNumber { get; private set; } = null!;
    public DateTime From { get; private set; }
    public DateTime? To { get; private set; }

    public Patient Patient { get; private set; } = null!;
    public InsurancePayer Payer { get; private set; } = null!;

    private PatientInsuranceHistory()
    {
    }

    public static PatientInsuranceHistory Create(
        Guid patientId,
        int sequenceNumber,
        string payerCode,
        string policyNumber,
        DateTime from,
        DateTime? to = null)
    {
        if (patientId == Guid.Empty)
            throw new ArgumentException("Patient id is required.", nameof(patientId));

        if (sequenceNumber <= 0)
            throw new ArgumentException("Sequence number must be greater than zero.", nameof(sequenceNumber));

        DateRange.EnsureValid(from, to);

        return new PatientInsuranceHistory
        {
            PatientId = patientId,
            SequenceNumber = sequenceNumber,
            PayerCode = Guard.Code(payerCode, nameof(payerCode)),
            PolicyNumber = Guard.NotEmpty(policyNumber, nameof(policyNumber)),
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
