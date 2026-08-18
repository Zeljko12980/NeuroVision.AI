using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace PdfService.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static Task SeedAsync(
        PdfDbContext context,
        ILogger? logger = null,
        CancellationToken cancellationToken = default) =>
        PdfTemplateSeeder.SeedAsync(
            context,
            logger ?? NullLogger.Instance,
            cancellationToken);
}
