namespace Gateway.API.Common.Model
{
    public sealed class ServiceEndpointsOptions
    {
        public Dictionary<string, string> Endpoints { get; set; } = new();
    }
}
