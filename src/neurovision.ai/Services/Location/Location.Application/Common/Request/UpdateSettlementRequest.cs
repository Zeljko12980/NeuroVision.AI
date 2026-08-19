
namespace LocationService.Application.Common.Request
{
    public class UpdateSettlementRequest
    {
        public string Name { get; set; } = null!;
        public string? PostalCode { get; set; }
    }
}
