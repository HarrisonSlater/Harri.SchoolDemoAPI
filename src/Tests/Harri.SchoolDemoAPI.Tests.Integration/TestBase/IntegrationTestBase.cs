using Microsoft.Extensions.Configuration;

namespace Harri.SchoolDemoAPI.Tests.Integration.TestBase
{
    [Category("Integration")]
    [TestFixture]
    public class IntegrationTestBase
    {
        public static string? SqlConnectionStringToTest { get; set; }

        static IntegrationTestBase()
        {
            var config = new ConfigurationBuilder().AddJsonFile("testappsettings.json").Build();
            SqlConnectionStringToTest = config["SqlConnectionStringToTest"];
        }
    }
}
