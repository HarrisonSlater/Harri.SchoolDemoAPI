using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http;
using System.Runtime.CompilerServices;

namespace Harri.SchoolDemoAPI.Results
{
    public static class StudentErrors
    {
        public static class StudentNotFound
        {
            public const string ErrorCode = "StudentNotFound";
            public static Error Error(int id) => new(ErrorCode, $"Student not found {id}");
        }

        public static class StudentUpdateConflict
        {
            public const string ErrorCode = "StudentUpdateConflict";
            public static Error Error(int id) => new(ErrorCode, $"Student {id} updated by another user");
        }

        public static class StudentRowVersionMismatch
        {
            public const string ErrorCode = "StudentRowVersionMismatch";
            public static Error Error(int id) => new(ErrorCode, $"Student {id} row version mismatch");
        }
    }
}
