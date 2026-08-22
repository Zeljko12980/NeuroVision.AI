using BuildingBlocks.Exceptions.Handler;

namespace DoctorService.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly CustomExceptionHandler _handler;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            CustomExceptionHandler handler,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _handler = handler;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unhandled doctor service exception. Path={Path}",
                    context.Request.Path);
                await _handler.TryHandleAsync(context, ex, CancellationToken.None);
            }
        }
    }
}