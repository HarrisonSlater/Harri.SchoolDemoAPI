using Harri.SchoolDemoAPI.Client;

namespace Harri.SchoolDemoAPI.Tests.E2E.TestBase
{
    public class StudentApiTestBase : E2ETestBase
    {
        protected static StudentApiClient _client;

        [OneTimeSetUp]
        public static void Setup()
        {
            if (APIUrlToTest is null) throw new ArgumentException("APIUrlToTest from appsettings.json cannot be null");

            _client = new StudentApiClient(new HttpClient() { BaseAddress = new Uri(APIUrlToTest) });
        }

        public async Task CleanUpTestStudent(int sId)
        {
            await _client.DeleteStudent(sId);
        }
    }
}
