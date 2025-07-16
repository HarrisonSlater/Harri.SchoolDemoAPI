// See: https://www.milanjovanovic.tech/blog/functional-error-handling-in-dotnet-with-the-result-pattern
// Extended with generic Value in ResultWith<T> 
using System;

namespace Harri.SchoolDemoAPI.Results
{
    public class ResultWith<T> : Result
    {
        private ResultWith(bool isSuccess, Error error, T value) : base(isSuccess, error)
        {
            Value = value;
        }

        private ResultWith(Result result) : base(result.IsSuccess, result.Error) { }
        private ResultWith(Result result, T value) : base(result.IsSuccess, result.Error) 
        {
            Value = value;
        }


        public static ResultWith<T> Success(T value) => new(true, Error.None, value);

        public static ResultWith<T> Failure(Error error, T value = default) => new(false, error, value);

        public static ResultWith<T> FromResult(Result result) => new(result);
        public static ResultWith<T> FromResult(Result result, T value) => new(result, value);

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
