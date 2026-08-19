
namespace LocationService.Application.Common.Response
{
    public class GovernmentHistoryResponse
    {
        public string CountryCode { get; set; } = null!;

        public int SequenceNumber { get; set; }

        public string GovernmentTypeCode { get; set; } = null!;

        public DateTime From { get; set; }

        public DateTime? To { get; set; }
    }
}
