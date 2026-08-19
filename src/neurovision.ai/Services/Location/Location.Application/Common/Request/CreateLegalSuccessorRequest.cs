
namespace LocationService.Application.Common.Request
{
    public class CreateLegalSuccessorRequest
    {
        public string SuccessorCountryCode { get; set; } = null!;
        public string PredecessorCountryCode { get; set; } = null!;
    }
}
