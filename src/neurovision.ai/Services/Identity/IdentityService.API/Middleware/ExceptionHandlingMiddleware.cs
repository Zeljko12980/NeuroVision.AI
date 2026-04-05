using BuildingBlocks.Exceptions.Handler;

namespace IdentityService.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly CustomExceptionHandler _handler;

        public ExceptionHandlingMiddleware(RequestDelegate next, CustomExceptionHandler handler)
        {
            _next = next;
            _handler = handler;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await _handler.TryHandleAsync(context, ex, CancellationToken.None);
            }
        }
    }
}
