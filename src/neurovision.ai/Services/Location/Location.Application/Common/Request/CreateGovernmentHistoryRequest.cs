
namespace LocationService.Application.Common.Request
{
    public class CreateGovernmentHistoryRequest
    {
        public string CountryCode { get; set; } = null!;
        public int SequenceNumber { get; set; }
        public string GovernmentTypeCode { get; set; } = null!;
        public DateTime From { get; set; }
        public DateTime? To { get; set; }
    }
}
