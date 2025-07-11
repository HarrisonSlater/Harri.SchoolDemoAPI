// See: https://www.milanjovanovic.tech/blog/functional-error-handling-in-dotnet-with-the-result-pattern
// Extended with array of onFailure
using System;
using System.Collections.Generic;

namespace Harri.SchoolDemoAPI.Results
{
    public static class ResultExtensions
    {
        public static T Match<T>(
            this Result result,
            Func<T> onSuccess,
            Func<Error, T> onFailure)
        {
            return result.IsSuccess ? onSuccess() : onFailure(result.Error);
        }
    }
}
