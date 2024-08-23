using Microsoft.Extensions.Configuration;

namespace Harri.SchoolDemoAPI.Tests.E2E.TestBase
{
    [Category("E2E")]
    [TestFixture]
    public class E2ETestBase
    {
        public static string? APIUrlToTest { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var config = new ConfigurationBuilder().AddJsonFile("testappsettings.json").Build();
            APIUrlToTest = config["APIUrlToTest"];
        }
    }
}
