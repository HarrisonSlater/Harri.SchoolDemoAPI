using HealthChecks.UI.Core;
using RestSharp;

namespace Harri.SchoolDemoAPI.HealthCheckClient
{
    public interface IHealthCheckApi
    {
        Task<UIHealthReport?> HealthCheck();
        Task<RestResponse<UIHealthReport>> HealthCheckRestResponse();
    }
}
