namespace Harri.SchoolDemoAPI.Tests.Integration
{
    [Category("Integration")]
    [TestFixture]

    public class IntegrationTestBase
    {
        protected static HostedProvider HostedProvider;

        [OneTimeSetUp]
        public static async Task OneTimeSetup()
        {
            HostedProvider = new HostedProvider();
            var t = new CancellationTokenSource();

            await HostedProvider.StartAsync(t.Token);
        }

        [OneTimeTearDown]
        public static void OneTimeTearDown()
        {
            HostedProvider.Dispose();
        }
    }
}
