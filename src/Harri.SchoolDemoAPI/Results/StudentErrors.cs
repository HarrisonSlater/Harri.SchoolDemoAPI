using Microsoft.AspNetCore.Http;

namespace Harri.SchoolDemoAPI.Results
{
    public static class StudentErrors
    {
        public static Error StudentNotFound(int id) => new("StudentNotFound", $"Student not found{id}");
    }
}
