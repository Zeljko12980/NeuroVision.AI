using System.ComponentModel.DataAnnotations;

namespace BuildingBlocks.Logging
{
    public sealed class LoggingOptions
    {
        public const string SectionName = "Observability";

        [Required]
        public string ServiceName { get; init; }

        [Required]
        public string LokiUrl { get; init; } 
    }
}
