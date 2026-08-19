
namespace LocationService.Application.Common.Response
{
    public class CapitalResponse
    {
        public string CountryCode { get; set; } = null!;

        public int SettlementCode { get; set; }

        public int SequenceNumber { get; set; }

        public DateTime From { get; set; }

        public DateTime? To { get; set; }
    }
}
