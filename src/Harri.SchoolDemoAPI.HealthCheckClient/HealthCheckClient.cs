using HealthChecks.UI.Core;
using RestSharp;
using System.Security.Cryptography;

namespace Harri.SchoolDemoAPI.HealthCheckClient
{
    /// <summary>
    /// .NET 8 REST Client for the Harri.SchoolDemoAPI /healthcheck endpoint using RestSharp.
    /// 
    /// Does not throw exceptions on failed requests.
    /// </summary>
    public class HealthCheckApiClient : IHealthCheckApi
    {
        private const string BaseRoute = "students/";
        private readonly RestClient _restClient;

        public HealthCheckApiClient(string uri)
        {
            var options = new RestClientOptions(uri);
            _restClient = new RestClient(options);
        }

        public HealthCheckApiClient(HttpClient httpClient)
        {
            _restClient = new RestClient(httpClient);
        }

        public HealthCheckApiClient(RestClient restClient)
        {
            _restClient = restClient;
        }

        public async Task<UIHealthReport?> HealthCheck()
        {
            var restResponse = await HealthCheckRestResponse();
            return restResponse.Data;
        }

        public async Task<RestResponse<UIHealthReport>> HealthCheckRestResponse()
        {
            var request = new RestRequest(BaseRoute + "healthcheck");
            var restResponse = await _restClient.ExecuteGetAsync<UIHealthReport>(request);
            //if (!restResponse.IsSuccessStatusCode)
            //{
            //    restResponse.Data = null;
            //}
            return restResponse;

        }
    }
}
