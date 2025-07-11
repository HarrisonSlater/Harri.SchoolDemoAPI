// See: https://www.milanjovanovic.tech/blog/functional-error-handling-in-dotnet-with-the-result-pattern
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

        public static T MatchError<T>(
            this Result result,
            Func<Error, T> errorSwitch)
        {
            return errorSwitch(result.Error);
        }
    }
}
