using HealthChecks.UI.Core;
using RestSharp;
using RestSharp.Serializers.Json;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Harri.SchoolDemoAPI.HealthCheckClient
{
    /// <summary>
    /// .NET 8 REST Client for the Harri.SchoolDemoAPI /healthcheck endpoint using RestSharp.
    /// 
    /// Does not throw exceptions on failed requests.
    /// </summary>
    public class HealthCheckApiClient : IHealthCheckApi
    {
        private const string BaseRoute = "";
        private readonly RestClient _restClient;

        public HealthCheckApiClient(string uri)
        {
            var options = new RestClientOptions(uri);

            var configureSerialization = new ConfigureSerialization(
                config => config.UseSystemTextJson(new JsonSerializerOptions(JsonSerializerDefaults.Web)
                {
                    Converters = { new JsonStringEnumConverter() }
                }
            ));
            
            _restClient = new RestClient(options, configureSerialization: configureSerialization);
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
            var request = new RestRequest(BaseRoute + "health");
            var restResponse = await _restClient.ExecuteGetAsync<UIHealthReport>(request);

            return restResponse;
        }
    }
}
