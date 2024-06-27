namespace Harri.SchoolDemoAPI.Tests.Integration
{
    [Category("Integration")]
    [TestFixture]

    public class IntegrationTestBase
    {
        public static string? APIUrlToTest {get;set;}

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            APIUrlToTest = config["APIUrlToTest"];
        }

        [OneTimeTearDown]
        public static void OneTimeTearDown()
        {

        }
    }
}
