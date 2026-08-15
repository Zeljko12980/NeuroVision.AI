using System.Net;

namespace BuildingBlocks.Results
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;

        public string Error { get; }
        public HttpStatusCode StatusCode { get; }

        protected Result(bool isSuccess, string error, HttpStatusCode statusCode)
        {
            if (isSuccess && !string.IsNullOrWhiteSpace(error))
                throw new ArgumentException("A successful result cannot contain an error message.");

            if (!isSuccess && string.IsNullOrWhiteSpace(error))
                throw new ArgumentException("A failed result must contain an error message.");

            IsSuccess = isSuccess;
            Error = error ?? string.Empty;
            StatusCode = statusCode;
        }

        public static Result Ok(HttpStatusCode statusCode = HttpStatusCode.OK)
            => new(true, string.Empty, statusCode);

        public static Result Created()
            => new(true, string.Empty, HttpStatusCode.Created);

        public static Result NoContent()
            => new(true, string.Empty, HttpStatusCode.NoContent);

        public static Result Fail(string error, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
            => new(false, error, statusCode);
    }

    public class Result<T> : Result
    {
        private readonly T? _value;

        public T Value =>
            IsSuccess
                ? _value!
                : throw new InvalidOperationException("Cannot access the value of a failed result.");

        protected internal Result(
            T? value,
            bool isSuccess,
            string error,
            HttpStatusCode statusCode)
            : base(isSuccess, error, statusCode)
        {
            _value = value;
        }

        public static Result<T> Ok(
            T value,
            HttpStatusCode statusCode = HttpStatusCode.OK)
            => new(value, true, string.Empty, statusCode);

        public static Result<T> Created(
            T value,
            HttpStatusCode statusCode = HttpStatusCode.Created)
            => new(value, true, string.Empty, statusCode);

        public static new Result<T> Fail(
            string error,
            HttpStatusCode statusCode = HttpStatusCode.BadRequest)
            => new(default, false, error, statusCode);

        public static Result<T> NoContent()
            => new(default, true, string.Empty, HttpStatusCode.NoContent);
    }
}