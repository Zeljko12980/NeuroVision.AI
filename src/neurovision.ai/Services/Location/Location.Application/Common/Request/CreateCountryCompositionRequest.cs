
namespace LocationService.Application.Common.Request
{
    public class CreateCountryCompositionRequest
    {
        public string UnionCountryCode { get; set; } = null!;
        public string MemberCountryCode { get; set; } = null!;
        public int SequenceNumber { get; set; }
        public DateTime From { get; set; }
        public DateTime? To { get; set; }
    }
}
