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
        private const string TestConsoleOutputFile = "ProviderTestsOutput.txt";

        [SetUp]
        public void SetUp()
        {
            _provider = new MockedHostedProvider();

            //This environment variable should only be set on build agents
            _providerPactPath = Environment.GetEnvironmentVariable("HARRI_PROVIDER_PACT_PATH");
            Console.WriteLine($"Provider Env Var: {_providerPactPath}");
            if (_providerPactPath != null )
            {
                _writer = new StreamWriter($"{TestContext.CurrentContext.WorkDirectory}/{TestConsoleOutputFile}");
                
                Console.SetOut(_writer);
            }
        }

        [TearDown]
        public void TearDown() 
        {
            _provider.Dispose();
            _writer.Close();
            if (_providerPactPath != null)
            {
                TestContext.AddTestAttachment($"{TestContext.CurrentContext.WorkDirectory}/{TestConsoleOutputFile}");
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
                                       "SchoolDempApi.Client-SchoolDemoApi.json");

            using var pactVerifier = new PactVerifier("SchoolDemoApi");

            pactVerifier
                .WithHttpEndpoint(_provider.ServerUri)
                .WithFileSource(new FileInfo(_providerPactPath == null ? localPactPath : _providerPactPath))
                .WithProviderStateUrl(new Uri(_provider.ServerUri, "/provider-states"))
                .Verify();
        }
    }
}
