using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Scalar.AspNetCore;

namespace Microsoft.Extensions.Hosting;

public static class ScalarApiReferenceExtensions
{
    public static WebApplication MapScalarApiReferenceIfAvailable(this WebApplication app, string title)
    {
        try
        {
            MapScalar(app, title);
        }
        catch (Exception ex) when (ex is FileLoadException or FileNotFoundException or BadImageFormatException)
        {
            var logger = app.Services.GetService<ILoggerFactory>()?.CreateLogger("Scalar");
            logger?.LogWarning(
                ex,
                "Scalar UI could not be loaded. OpenAPI is still at /openapi/v1.json. Windows Smart App Control often blocks unsigned Scalar.AspNetCore.dll.");
        }

        return app;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void MapScalar(WebApplication app, string title)
    {
        app.MapScalarApiReference(options =>
        {
            options
                .WithTitle(title)
                .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
        });
    }
}
