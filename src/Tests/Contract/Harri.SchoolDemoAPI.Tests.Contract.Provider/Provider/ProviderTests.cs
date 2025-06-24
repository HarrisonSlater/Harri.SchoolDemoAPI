using PactNet.Verifier;

namespace Harri.SchoolDemoAPI.Tests.Contract.Provider
{
    [TestFixture]
    [Category("Provider")]
    public class ProviderTests
    {
        private MockedHostedProvider _provider;
        private TextWriter? _writer;
        private string? _providerPactPath;

        //This file is visible under attachments on the VerifySchoolDemoAPIHonoursPactsWithConsumer test run in azure devops
        private const string ProviderTestsOutputFileName = "ProviderTestsOutput.txt";
        private string ProviderTestOutputFullPath => $"{TestContext.CurrentContext.WorkDirectory}/{ProviderTestsOutputFileName}";

        [SetUp]
        public void SetUp()
        {
            _provider = new MockedHostedProvider();

            //This environment variable should only be set on build agents
            _providerPactPath = Environment.GetEnvironmentVariable("HARRI_PROVIDER_PACT_PATH");
            if (_providerPactPath != null )
            {
                _writer = new StreamWriter(ProviderTestOutputFullPath);
                
                Console.SetOut(_writer);
            }
        }

        [TearDown]
        public void TearDown() 
        {
            _provider.Dispose();

            if (_providerPactPath != null)
            {
                _writer?.Close();
                TestContext.AddTestAttachment(ProviderTestOutputFullPath);
            }
        }

        [Test]
        public void VerifySchoolDemoAPIHonoursPactsWithConsumer()
        {
            string localPactPath = Path.Combine("..",
                                       "..",
                                       "..",
                                       "..",
                                       "pacts",
                                       "SchoolDemoApi.Client-SchoolDemoApi.json");

            using var pactVerifier = new PactVerifier("SchoolDemoApi");

            pactVerifier
                .WithHttpEndpoint(_provider.ServerUri)
                .WithFileSource(new FileInfo(_providerPactPath == null ? localPactPath : _providerPactPath))
                .WithProviderStateUrl(new Uri(_provider.ServerUri, "/provider-states"))
                .Verify();
        }
    }
}
