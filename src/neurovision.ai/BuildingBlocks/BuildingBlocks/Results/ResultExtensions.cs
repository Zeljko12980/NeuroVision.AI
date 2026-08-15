using System.Net;

namespace BuildingBlocks.Results;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(
        this Result<T> result,
        Func<T, IActionResult>? onSuccess = null)
    {
        if (!result.IsSuccess)
        {
            return CreateProblem(
                result.StatusCode,
                result.Error);
        }

        if (onSuccess is not null)
        {
            return onSuccess(result.Value);
        }

        return new ObjectResult(result.Value)
        {
            StatusCode = (int)result.StatusCode
        };
    }


    public static IActionResult ToActionResult(
        this Result result)
    {
        if (!result.IsSuccess)
        {
            return CreateProblem(
                result.StatusCode,
                result.Error);
        }

        return new StatusCodeResult(
            (int)result.StatusCode);
    }


    public static Result<TOut> Map<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, TOut> mapper)
    {
        if (!result.IsSuccess)
        {
            return Result<TOut>.Fail(
                result.Error,
                result.StatusCode);
        }

        return Result<TOut>.Ok(
            mapper(result.Value),
            result.StatusCode);
    }


    private static ObjectResult CreateProblem(
        HttpStatusCode statusCode,
        string error)
    {
        var problem = new ProblemDetails
        {
            Title = GetTitle(statusCode),
            Detail = error,
            Status = (int)statusCode,
            Instance = Activity.Current?.Id
        };

        return new ObjectResult(problem)
        {
            StatusCode = (int)statusCode,
            ContentTypes =
            {
                "application/problem+json"
            }
        };
    }


    private static string GetTitle(
        HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.BadRequest => "Bad Request",
            HttpStatusCode.Unauthorized => "Unauthorized",
            HttpStatusCode.Forbidden => "Forbidden",
            HttpStatusCode.NotFound => "Not Found",
            HttpStatusCode.Conflict => "Conflict",
            HttpStatusCode.InternalServerError => "Internal Server Error",
            _ => "Request Failed"
        };
    }
}