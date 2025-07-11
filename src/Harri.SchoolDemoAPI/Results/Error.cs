// See: https://www.milanjovanovic.tech/blog/functional-error-handling-in-dotnet-with-the-result-pattern
namespace Harri.SchoolDemoAPI.Results
{
    public sealed record Error(string Code, string Description)
    {
        public static readonly Error None = new(string.Empty, string.Empty);
    }
}
