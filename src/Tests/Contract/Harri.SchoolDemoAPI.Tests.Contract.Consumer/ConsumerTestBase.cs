using PactNet;
using System.Text.Json;

namespace Harri.SchoolDemoAPI.Tests.Contract.Consumer
{
    [TestFixture]
    [Category("Consumer")]
    public class ConsumerTestBase
    {
        protected IPactBuilderV4 _pact;

        [OneTimeSetUp]
        [SetUp]
        public void Setup()
        {
            var config = new PactConfig()
            {
                PactDir = "../../../../pacts/",
                DefaultJsonSettings = new JsonSerializerOptions
                {
                }
            };

            IPactV4 pact = Pact.V4("SchoolDemoApi.Client", "SchoolDemoApi", config);

            _pact = pact.WithHttpInteractions();
        }
    }
}
