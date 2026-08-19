
namespace LocationService.Application.Common.Request
{
    public class UpdateGovernmentHistoryRequest
    {
        public string GovernmentTypeCode { get; set; } = null!;
        public DateTime From { get; set; }
        public DateTime? To { get; set; }
    }
}
