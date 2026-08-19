
namespace LocationService.Application.Common.Request
{
    public class CreateCapitalRequest
    {
        public string CountryCode { get; set; } = null!;
        public int SettlementCode { get; set; }
        public int SequenceNumber { get; set; }
        public DateTime From { get; set; }
        public DateTime? To { get; set; }
    }
}
