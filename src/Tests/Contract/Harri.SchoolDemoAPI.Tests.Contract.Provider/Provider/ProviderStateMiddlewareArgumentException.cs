using System.Runtime.Serialization;

namespace Harri.SchoolDemoAPI.Tests.Contract.Provider
{
    public class ProviderStateMiddlewareArgumentException : ArgumentException
    {
        public ProviderStateMiddlewareArgumentException()
        {
        }

        public ProviderStateMiddlewareArgumentException(string? message) : base(message)
        {
        }

        public ProviderStateMiddlewareArgumentException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
