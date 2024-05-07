using PactNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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

            IPactV4 pact = Pact.V4("SchoolDempApi.Client", "SchoolDemoApi", config);

            _pact = pact.WithHttpInteractions();
        }
    }
}
