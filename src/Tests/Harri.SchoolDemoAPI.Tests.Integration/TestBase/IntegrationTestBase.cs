using Microsoft.Extensions.Configuration;

namespace Harri.SchoolDemoAPI.Tests.Integration.TestBase
{
    [Category("Integration")]
    [TestFixture]
    public class IntegrationTestBase
    {
        public static string? SqlConnectionStringToTest { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUpBase()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            SqlConnectionStringToTest = config["SqlConnectionStringToTest"];
        }
    }
}
