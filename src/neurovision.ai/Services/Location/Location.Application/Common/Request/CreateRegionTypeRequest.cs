
namespace LocationService.Application.Common.Request
{
    public class CreateRegionTypeRequest
    {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}
