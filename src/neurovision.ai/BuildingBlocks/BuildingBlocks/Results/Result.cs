namespace BuildingBlocks.Results
{
    public class Result
    {
        public bool IsSuccess { get; private set; }
        public bool IsFailure => !IsSuccess;
        public string Error { get; private set; } = string.Empty;

        protected Result(bool isSuccess, string error)
        {
            if (isSuccess && !string.IsNullOrEmpty(error))
                throw new InvalidOperationException("Success result cannot have an error message.");

            if (!isSuccess && string.IsNullOrEmpty(error))
                throw new InvalidOperationException("Failure result must have an error message.");

            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Fail(string error) => new Result(false, error);
        public static Result Ok() => new Result(true, string.Empty);
    }

    public class Result<T> : Result
    {
        private readonly T _value;
        public T Value
        {
            get
            {
                if (!IsSuccess)
                    throw new InvalidOperationException("Cannot get the value of a failed result.");
                return _value;
            }
        }

        protected internal Result(T value, bool isSuccess, string error)
            : base(isSuccess, error)
        {
            _value = value;
        }

        public static Result<T> Ok(T value) => new Result<T>(value, true, string.Empty);
        public new static Result<T> Fail(string error) => new Result<T>(default!, false, error);
    }
}
