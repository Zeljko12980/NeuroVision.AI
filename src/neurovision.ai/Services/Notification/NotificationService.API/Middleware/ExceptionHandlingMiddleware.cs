using BuildingBlocks.Exceptions.Handler;

namespace NotificationService.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate next;
    private readonly CustomExceptionHandler handler;
    private readonly ILogger<ExceptionHandlingMiddleware> logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        CustomExceptionHandler handler,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        this.next = next;
        this.handler = handler;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unhandled notification service exception. Path={Path}",
                context.Request.Path);
            await handler.TryHandleAsync(context, ex, CancellationToken.None);
        }
    }
}
