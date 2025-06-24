// No namespace means this will run once for this assembly
[SetUpFixture]
public class SetUpFixture
{
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        // Delete any pre-existing pact file before any consumer tests run
        var fileName = "../../../../pacts/SchoolDemoApi.Client-SchoolDemoApi.json";
        if(File.Exists(fileName))
        {
            File.Delete(fileName);
        }
    }
}
