// See: https://www.milanjovanovic.tech/blog/functional-error-handling-in-dotnet-with-the-result-pattern
// Extended with generic Value
using System;

namespace Harri.SchoolDemoAPI.Results
{
    public class Result<T> : Result
    {
        private Result(bool isSuccess, Error error, T value) : base(isSuccess, error)
        {
            Value = value;
        }

        public static Result<T> Success(T value) => new(true, Error.None, value);

        public static Result<T> Failure(Error error, T value = default) => new(false, error, value);

        public T Value { get; private set; }
    }

    public class Result
    {
        protected Result(bool isSuccess, Error error)
        {
            if (isSuccess && error != Error.None ||
                !isSuccess && error == Error.None)
            {
                throw new ArgumentException("Invalid error", nameof(error));
            }

            IsSuccess = isSuccess;
            Error = error;
        }

        public bool IsSuccess { get; }

        public bool IsFailure => !IsSuccess;

        public Error Error { get; }

        public static Result Success() => new(true, Error.None);

        public static Result Failure(Error error) => new(false, error);
    }
}
