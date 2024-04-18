using PactNet.Verifier;

namespace Harri.SchoolDemoAPI.Tests.Contract.Provider
{
    [TestFixture]
    [Category("Provider")]
    public class ProviderTests
    {
        MockedHostedProvider _provider;

        [SetUp]
        public void SetUp()
        {
            _provider = new MockedHostedProvider();
        }

        [TearDown]
        public void TearDown() 
        {
            _provider.Dispose();
        }

        [Test]
        public void VerifySchoolDemoAPIHonoursPactsWithConsumer()
        {
            var envVar = Environment.GetEnvironmentVariable("HARRI_PROVIDER_PACT_PATH", EnvironmentVariableTarget.User);
            string pactPath = Path.Combine("..",
                                       "..",
                                       "..",
                                       "..",
                                       "pacts",
                                       "SchoolDempApi.Client-SchoolDemoApi.json");

            using var pactVerifier = new PactVerifier("SchoolDemoApi");

            pactVerifier
                .WithHttpEndpoint(_provider.ServerUri)
                .WithFileSource(new FileInfo(envVar == null ? pactPath : envVar))
                .WithProviderStateUrl(new Uri(_provider.ServerUri, "/provider-states"))
                .Verify();
        }
    }
}
