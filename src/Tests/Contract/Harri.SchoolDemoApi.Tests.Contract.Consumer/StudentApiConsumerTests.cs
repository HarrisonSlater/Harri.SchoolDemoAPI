using FluentAssertions;
using Harri.SchoolDemoApi.Client;
using PactNet;
using PactNet.Matchers;
using System.Net;
using System.Text.Json;

namespace Harri.SchoolDemoAPI.Tests.Contract.Consumer
{
    [TestFixture]
    [Category("Consumer")]
    public class StudentApiConsumerTests
    {
        private IPactBuilderV4 _pact;

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
        //[TestCase(1, "Mocky", 3)]
        //[TestCase(12, "Mocky McMockName", 3.80)]
        //[TestCase(123, "Mocky McMockName", 3.999)]
        //[TestCase(1234, "Mocky McMockName", 3.81)]
        [TestCase(4567, "Mocky McMockName", null)]

        public async Task GetStudent_WhenCalled_ReturnsAStudent(int sId, string name, decimal? GPA)
        {
            _pact.UponReceiving($"a request to get a student with id {sId}")
                    .Given("a student with sId {sId} exists", new Dictionary<string, string>() {
                        { "sId", sId.ToString() },
                        { "name", name },
                        { "GPA", GPA is null ? "null" : GPA.ToString() },
                    })
                    .WithRequest(HttpMethod.Get, $"/student/{sId}")
                  //.WithHeader("Accept", "application/json")
                 .WillRespond()
                 .WithStatus(HttpStatusCode.OK)
                 .WithHeader("Content-Type", "application/json; charset=utf-8")
                 .WithJsonBody(new
                 {
                     sId = Match.Equality(sId),
                     name = Match.Equality(name),
                     GPA = Match.Equality(GPA)
                 });

            await _pact.VerifyAsync(async ctx =>
            {
                var client = new StudentApiClient(ctx.MockServerUri.ToString());
                var student = await client.GetStudent(sId);

                // Client Assertions
                student.Should().NotBeNull();

                student.SId.Should().NotBeNull().And.Be(sId);
                student.Name.Should().NotBeNullOrEmpty().And.Be(name);
                student.GPA.Should().Be(GPA);
            });

        }
    }
}