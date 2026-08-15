namespace BuildingBlocks.Behaviors
{
    public sealed class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    {
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            logger.LogInformation(
                "Handling {Request} {@Request}",
                typeof(TRequest).Name,
                request);

            try
            {
                var response = await next();

                stopwatch.Stop();

                logger.LogInformation(
                    "Handled {Request} in {ElapsedMilliseconds} ms",
                    typeof(TRequest).Name,
                    stopwatch.ElapsedMilliseconds);

                if (stopwatch.ElapsedMilliseconds > 3000)
                {
                    logger.LogWarning(
                        "{Request} took {ElapsedMilliseconds} ms",
                        typeof(TRequest).Name,
                        stopwatch.ElapsedMilliseconds);
                }

                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                logger.LogError(
                    ex,
                    "{Request} failed after {ElapsedMilliseconds} ms",
                    typeof(TRequest).Name,
                    stopwatch.ElapsedMilliseconds);

                throw;
            }
        }
    }
}
