namespace Gateway.API.Common.Response
{
    public sealed class ServiceHealthResponse
    {
        public string Name { get; set; }
        public string Status { get; set; } 
        public string? Error { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
