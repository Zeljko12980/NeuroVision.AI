
namespace LocationService.Application.Common.Request
{
    public class UpdateLocalCommunityRequest
    {
        public string Name { get; set; } = null!;
        public int? OfficeSettlementCode { get; set; }
    }
}
