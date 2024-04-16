using FluentAssertions;
using Harri.SchoolDemoApi.Client;
using PactNet;
using PactNet.Matchers;
using System.Net;
using System.Security.Cryptography;
using System.Text.Json;
using System.Xml.Linq;
using Harri.SchoolDemoAPI.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net.Http.Headers;

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

        [TestCase(1, "Mocky", 3)]
        [TestCase(1234, "Mocky McMockName", 3.81)]
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

        [Test]
        public async Task GetStudent_WhenCalledWithNonExistantStudentId_Returns404()
        {
            var sId = 0404;
            _pact.UponReceiving($"a request to get a student with id {sId}")
                    .Given("a student with sId {sId} does not exist", new Dictionary<string, string>() {
                        { "sId", sId.ToString() }
                    })
                    .WithRequest(HttpMethod.Get, $"/student/{sId}")
                 .WillRespond()
                 .WithStatus(HttpStatusCode.NotFound)
                 .WithJsonBody(new
                 {
                     title = Match.Type("error title"),
                     status = Match.Equality((int)HttpStatusCode.NotFound)
                 });

            await _pact.VerifyAsync(async ctx =>
            {
                var client = new StudentApiClient(ctx.MockServerUri.ToString());
                var studentResponse = await client.GetStudentRestResponse(sId);

                // Client Assertions
                studentResponse.Data.Should().BeNull();
                studentResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            });

        }

        [TestCase(-1000)]
        public async Task GetStudent_WhenCalledWithInvalidStudentId_Returns400(int sId)
        {
            _pact.UponReceiving($"a request to get a student with an invalid id")
                .WithRequest(HttpMethod.Get, $"/student/{sId}")
                .WillRespond()
                .WithStatus(HttpStatusCode.BadRequest)
                .WithJsonBody(new
                {
                    title = Match.Type("title"),
                    status = Match.Equality(400),
                    errors = new
                    {
                        sId = new dynamic[] { Match.Type("error message") }
                    }
                });

            await _pact.VerifyAsync(async ctx =>
            {
                var client = new StudentApiClient(ctx.MockServerUri.ToString());
                var studentResponse = await client.GetStudentRestResponse(sId);

                // Client Assertions
                studentResponse.Data.Should().BeNull();
                studentResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
                studentResponse.ShouldContainErrorMessageForProperty("sId");
            });

        }

        [TestCase(1234, "Mocky McMockName", 3.81)]
        [TestCase(4567, "Mocky", null)]
        public async Task AddStudent_WhenCalledWithNewStudent_ReturnsSuccess_AndAddsANewStudent(int sIdNew, string name, decimal? GPA)
        {
            _pact.UponReceiving($"a request to add a new student with name {name}")
                    .Given("a student with sId {sIdNew} will be created", new Dictionary<string, string>() {
                        {"sIdNew", sIdNew.ToString() },
                        { "name", name },
                        { "GPA", GPA is null ? "null" : GPA.ToString() },
                    })
                    .WithRequest(HttpMethod.Post, $"/student/")
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithJsonBody(Match.Equality(new {
                        name = name,
                        GPA = GPA
                    }))
                 .WillRespond()
                 .WithStatus(HttpStatusCode.OK)
                 .WithHeader("Content-Type", "application/json; charset=utf-8")
                 .WithJsonBody(Match.Equality(sIdNew));

            await _pact.VerifyAsync(async ctx =>
            {
                var client = new StudentApiClient(ctx.MockServerUri.ToString());
                var student = await client.AddStudent(new NewStudent() { Name = name, GPA = GPA });

                // Client Assertions
                student.Should().NotBeNull().And.Be(sIdNew);
            });
        }

        [TestCase(4567, null, null)]
        [TestCase(4567, "", null)]
        [TestCase(4567, "  \t\r\n   ", null)]
        public async Task AddStudent_WhenCalledWithInvalidNewStudent_Returns400(int sIdNew, string? name, decimal? GPA)
        {
            _pact.UponReceiving($"a request to add a new student without a name")
                    .WithRequest(HttpMethod.Post, $"/student/")
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithJsonBody(Match.Equality(new
                    {
                        name = name,
                        GPA = GPA
                    }))
                 .WillRespond()
                 .WithStatus(HttpStatusCode.BadRequest)
                 .WithHeader("Content-Type", "application/json; charset=utf-8")
                 .WithJsonBody(new
                 {
                     title = Match.Type("title"),
                     status = Match.Equality(400),
                     errors = new
                     {
                         name = new dynamic[] { Match.Type("error message") }
                     }
                 });

            await _pact.VerifyAsync(async ctx =>
            {
                var client = new StudentApiClient(ctx.MockServerUri.ToString());
                var response = await client.AddStudentRestResponse(new NewStudent() { Name = name, GPA = GPA });

                // Client Assertions
                response.Data.Should().BeNull();
                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

                response.ShouldContainErrorMessageForProperty("name");
            });
        }

        [TestCase(4567, "Mocky", null)]
        [TestCase(123, "Mocky", 3.81)]
        public async Task UpdateStudent_WhenCalledWithValidStudent_ReturnsSuccess_AndUpdatesStudent(int sId, string? name, decimal? GPA)
        {
            _pact.UponReceiving($"a request to update a student with sId {sId}")
                    .Given("a student with sId {sId} exists and will be updated", new Dictionary<string, string>() {
                        {"sId", sId.ToString() },
                        { "name", name },
                        { "GPA", GPA is null ? "null" : GPA.ToString() },
                    })
                    .WithRequest(HttpMethod.Put, $"/student/")
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithJsonBody(Match.Equality(new
                    {
                        sId = sId,
                        name = name,
                        GPA = GPA
                    }))
                 .WillRespond()
                 .WithStatus(HttpStatusCode.OK)
                 .WithHeader("Content-Type", "application/json; charset=utf-8")
                 .WithJsonBody(Match.Equality(true));

            await _pact.VerifyAsync(async ctx =>
            {
                var client = new StudentApiClient(ctx.MockServerUri.ToString());
                var student = await client.UpdateStudent(new Student() { SId = sId, Name = name, GPA = GPA });

                // Client Assertions
                student.Should().Be(true);
            });
        }

        [TestCase(1234, null, 3.81)]
        [TestCase(4567, "", 3.81)]
        public async Task UpdateStudent_WhenCalledWithAnInvalidRequest_Returns400(int sId, string? name, decimal? GPA)
        {
            _pact.UponReceiving($"a request to update a student with sId {sId} and invalid name")
                .Given("no student will be updated")
                    .WithRequest(HttpMethod.Put, $"/student/")
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithJsonBody(Match.Equality(new
                    {
                        sId = sId,
                        name = name,
                        GPA = GPA
                    }))
                 .WillRespond()
                 .WithStatus(HttpStatusCode.BadRequest)
                 .WithHeader("Content-Type", "application/json; charset=utf-8")
                 .WithJsonBody(new
                 {
                     title = Match.Type("title"),
                     status = Match.Equality(400),
                     errors = new
                     {
                         name = new dynamic[] { Match.Type("error message") }
                     }
                 });

            await _pact.VerifyAsync(async ctx =>
            {
                var client = new StudentApiClient(ctx.MockServerUri.ToString());
                var response = await client.UpdateStudentRestResponse(new Student() { SId = sId, Name = name, GPA = GPA });

                // Client Assertions
                response.Data.Should().BeNull();
                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

                response.ShouldContainErrorMessageForProperty("name");
            });
        }

        [TestCase(4567, "Mocky Mockson", 3.81)]
        public async Task UpdateStudent_WhenCalledWithNonExistantStudentId_Returns404(int sId, string? name, decimal? GPA)
        {
            _pact.UponReceiving($"a request to update a student with sId {sId} and invalid name")
                .Given("a student with sId {sId} does not exist")
                    .WithRequest(HttpMethod.Put, $"/student/")
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithJsonBody(Match.Equality(new
                    {
                        sId = sId,
                        name = name,
                        GPA = GPA
                    }))
                 .WillRespond()
                 .WithStatus(HttpStatusCode.NotFound)
                 .WithHeader("Content-Type", "application/json; charset=utf-8")
                 .WithJsonBody(Match.Equality(false));

            await _pact.VerifyAsync(async ctx =>
            {
                var client = new StudentApiClient(ctx.MockServerUri.ToString());
                var response = await client.UpdateStudentRestResponse(new Student() { SId = sId, Name = name, GPA = GPA });

                // Client Assertions
                response.Data.Should().BeFalse();
                response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            });
        }

        [TestCase(123)]
        [TestCase(456)]
        public async Task DeleteStudent_WhenCalledWithValidStudentId_ReturnsSuccess_AndDeletesStudent(int sId)
        {
            _pact.UponReceiving($"a request to delete a student with sId {sId}")
                    .Given("a student with sId {sId} exists and will be deleted", new Dictionary<string, string>() {
                        {"sId", sId.ToString() },
                    })
                    .WithRequest(HttpMethod.Delete, $"/student/{sId}")
                 .WillRespond()
                 .WithStatus(HttpStatusCode.OK)
                 .WithHeader("Content-Type", "application/json; charset=utf-8")
                 .WithJsonBody(Match.Equality(true));

            await _pact.VerifyAsync(async ctx =>
            {
                var client = new StudentApiClient(ctx.MockServerUri.ToString());
                var student = await client.DeleteStudent(sId);

                // Client Assertions
                student.Should().Be(true);
            });
        }

        [TestCase(1122)]
        public async Task DeleteStudent_WhenCalledWithANonExistantStudentId_Returns404(int sId)
        {
            _pact.UponReceiving($"a request to delete a student with sId {sId}")
                    .Given("a student with sId {sId} does not exist and will not be deleted", new Dictionary<string, string>() {
                        {"sId", sId.ToString() },
                    })
                    .WithRequest(HttpMethod.Delete, $"/student/{sId}")
                 .WillRespond()
                 .WithStatus(HttpStatusCode.NotFound)
                 .WithHeader("Content-Type", "application/json; charset=utf-8")
                 .WithJsonBody(Match.Equality(false));

            await _pact.VerifyAsync(async ctx =>
            {
                var client = new StudentApiClient(ctx.MockServerUri.ToString());
                var student = await client.DeleteStudent(sId);

                // Client Assertions
                student.Should().Be(false);
            });
        }

        public async Task DeleteStudent_WhenCalledWithValidStudentIdWithExistingApplications_Returns409(int sId)
        {
            _pact.UponReceiving($"a request to delete a student with sId {sId}")
                    .Given("a student with sId { sId } exists but can not be deleted", new Dictionary<string, string>() {
                        {"sId", sId.ToString() },
                    })
                    .WithRequest(HttpMethod.Delete, $"/student/{sId}")
                 .WillRespond()
                 .WithStatus(HttpStatusCode.Conflict)
                 .WithHeader("Content-Type", "application/json; charset=utf-8")
                 .WithJsonBody(Match.Equality(false));

            await _pact.VerifyAsync(async ctx =>
            {
                var client = new StudentApiClient(ctx.MockServerUri.ToString());
                var student = await client.DeleteStudent(sId);

                // Client Assertions
                student.Should().Be(false);
            });
        }

        [TestCase(-1234)]
        public async Task DeleteStudent_WhenCalledWithAnInvalidRequest_Returns400(int sId)
        {
            _pact.UponReceiving($"a request to delete a student with invalid sId {sId}")
                .Given("no student will be deleted")
                    .WithRequest(HttpMethod.Delete, $"/student/{sId}")
                 .WillRespond()
                 .WithStatus(HttpStatusCode.BadRequest)
                 .WithHeader("Content-Type", "application/json; charset=utf-8")
                 .WithJsonBody(new
                 {
                     title = Match.Type("title"),
                     status = Match.Equality(400),
                     errors = new
                     {
                         sId = new dynamic[] { Match.Type("error message") }
                     }
                 });

            await _pact.VerifyAsync(async ctx =>
            {
                var client = new StudentApiClient(ctx.MockServerUri.ToString());
                var response = await client.DeleteStudentRestResponse(sId);

                // Client Assertions
                response.Data.Should().BeFalse();
                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

                response.ShouldContainErrorMessageForProperty("sId");
            });
        }

    }
}