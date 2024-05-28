namespace Harri.SchoolDemoAPI.Tests.Integration
{
    [Category("Integration")]
    [TestFixture]

    public class IntegrationTestBase
    {
        public static string? APIUrlToTest {get;set;}

        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            APIUrlToTest = config["APIUrlToTest"];
            throw new Exception($"APIUrlToTest: {APIUrlToTest}");
        }

        [OneTimeTearDown]
        public static void OneTimeTearDown()
        {

        }
    }
}
