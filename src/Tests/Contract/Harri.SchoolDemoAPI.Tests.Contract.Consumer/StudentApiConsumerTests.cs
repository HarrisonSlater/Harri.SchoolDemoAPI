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
using Harri.SchoolDemoAPI.Models.Dto;

namespace Harri.SchoolDemoAPI.Tests.Contract.Consumer
{
    public class StudentApiConsumerTests : ConsumerTestBase
    {
        [TestCase(1, "Mocky", 3)]
        [TestCase(1234, "Mocky McMockName", 3.81)]
        [TestCase(4567, "Mocky McMockName", null)]

        public async Task GetStudent_WhenCalled_ReturnsAStudent(int sId, string name, decimal? GPA)
        {
            _pact.UponReceiving($"a request to get a student with id {sId}")
                    .Given("a student with sId {sId} exists", new Dictionary<string, string>() {
                        { "sId", sId.ToString() },
                        { "name", name },
                        { "GPA", GPA?.ToString() ?? "null" },
                    })
                    .WithRequest(HttpMethod.Get, $"/students/{sId}")
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
                    .WithRequest(HttpMethod.Get, $"/students/{sId}")
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
                .WithRequest(HttpMethod.Get, $"/students/{sId}")
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
                        { "GPA", GPA?.ToString() ?? "null" },
                    })
                    .WithRequest(HttpMethod.Post, $"/students/")
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
                var student = await client.AddStudent(new NewStudentDto() { Name = name, GPA = GPA });

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
                    .WithRequest(HttpMethod.Post, $"/students/")
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
                var response = await client.AddStudentRestResponse(new NewStudentDto() { Name = name, GPA = GPA });

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
                        { "GPA", GPA?.ToString() ?? "null" },
                    })
                    .WithRequest(HttpMethod.Put, $"/students/{sId}")
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithJsonBody(Match.Equality(new
                    {
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
                var student = await client.UpdateStudent(sId, new UpdateStudentDto() { Name = name, GPA = GPA });

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
                    .WithRequest(HttpMethod.Put, $"/students/{sId}")
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
                var response = await client.UpdateStudentRestResponse(sId, new UpdateStudentDto() { Name = name, GPA = GPA });

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
                    .WithRequest(HttpMethod.Put, $"/students/{sId}")
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithJsonBody(Match.Equality(new
                    {
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
                var response = await client.UpdateStudentRestResponse(sId,new UpdateStudentDto() { Name = name, GPA = GPA });

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
                    .WithRequest(HttpMethod.Delete, $"/students/{sId}")
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
                    .WithRequest(HttpMethod.Delete, $"/students/{sId}")
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
                    .WithRequest(HttpMethod.Delete, $"/students/{sId}")
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
                    .WithRequest(HttpMethod.Delete, $"/students/{sId}")
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

        // PatchStudent
        //[TestCase(4567, "Mocky", null)]
        [TestCase(123, "Mocky")]
        public async Task PatchStudent_WhenCalledWithValidStudent_ReturnsSuccess_AndPatchesStudentName(int sId, string newName)
        {
            var existingName = "EXISTING STUDENT NAME";
            var existingGPA = 3.91m;
            _pact.UponReceiving($"a request to patch a student with sId {sId}")
                    .Given("a student with sId {sId} exists", new Dictionary<string, string>() {
                        {"sId", sId.ToString() },
                        { "name", existingName },
                        { "GPA", existingGPA.ToString() },
                    })
                    .Given("a student with sId {sId} will be updated", new Dictionary<string, string>() {
                        {"sId", sId.ToString() },
                        { "name", newName },
                        { "GPA", existingGPA.ToString() },
                    })
                    .WithRequest(HttpMethod.Patch, $"/students/{sId}")
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithJsonBody(Match.Equality(new
                    {
                        name = newName
                    }))
                 .WillRespond()
                 .WithStatus(HttpStatusCode.OK)
                 .WithHeader("Content-Type", "application/json; charset=utf-8")
                 .WithJsonBody(new
                 {
                     sId = Match.Equality(sId),
                     name = Match.Equality(newName),
                     GPA = Match.Equality(existingGPA)
                 });

            await _pact.VerifyAsync(async ctx =>
            {
                var client = new StudentApiClient(ctx.MockServerUri.ToString());
                var student = await client.PatchStudent(sId, new StudentPatchDto() { Name = newName });

                // Client Assertions
                student.Should().BeEquivalentTo(new StudentDto()
                {
                    SId = sId,
                    Name = newName,
                    GPA = existingGPA
                });
            });
        }

        [TestCase(123, 3.52)]
        public async Task PatchStudent_WhenCalledWithValidStudent_ReturnsSuccess_AndPatchesStudentGPA(int sId, decimal gpa)
        {
            var existingName = "EXISTING STUDENT NAME";
            var existingGPA = 3.91m;
            _pact.UponReceiving($"a request to patch a student with sId {sId}")
                    .Given("a student with sId {sId} exists", new Dictionary<string, string>() {
                        {"sId", sId.ToString() },
                        { "name", existingName },
                        { "GPA", existingGPA.ToString() },
                    })
                    .Given("a student with sId {sId} will be updated", new Dictionary<string, string>() {
                        {"sId", sId.ToString() },
                        { "name", existingName },
                        { "GPA", gpa.ToString() },
                    })
                    .WithRequest(HttpMethod.Patch, $"/students/{sId}")
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithJsonBody(Match.Equality(new
                    {
                        GPA = gpa
                    }))
                 .WillRespond()
                 .WithStatus(HttpStatusCode.OK)
                 .WithHeader("Content-Type", "application/json; charset=utf-8")
                 .WithJsonBody(new
                 {
                     sId = Match.Equality(sId),
                     name = Match.Equality(existingName),
                     GPA = Match.Equality(gpa)
                 });

            await _pact.VerifyAsync(async ctx =>
            {
                var client = new StudentApiClient(ctx.MockServerUri.ToString());
                var student = await client.PatchStudent(sId, new StudentPatchDto() { GPA = gpa });

                // Client Assertions
                student.Should().BeEquivalentTo(new StudentDto()
                {
                    SId = sId,
                    Name = existingName,
                    GPA = gpa
                });
            });
        }

        
        [TestCase(1234, null, 3.81)]
        [TestCase(4567, "", 3.81)]
        public async Task PatchStudent_WhenCalledWithAnInvalidRequest_Returns400(int sId, string? name, decimal? GPA)
        {
            _pact.UponReceiving($"a request to patch a student with sId {sId} and invalid name")
                .Given("no student will be updated")
                    .WithRequest(HttpMethod.Patch, $"/students/{sId}")
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
                var response = await client.PatchStudentRestResponse(sId, new StudentPatchDto() { Name = name, GPA = GPA });

                // Client Assertions
                response.Data.Should().BeNull();
                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

                response.ShouldContainErrorMessageForProperty("name");
            });
        }
        
        [TestCase(4567, "Mocky Mockson", 3.81)]
        public async Task PatchStudent_WhenCalledWithNonExistantStudentId_Returns404(int sId, string? name, decimal? GPA)
        {
            _pact.UponReceiving($"a request to patch a non-existant student with sId {sId}")
                .Given("a student with sId {sId} does not exist")
                    .WithRequest(HttpMethod.Patch, $"/students/{sId}")
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithJsonBody(Match.Equality(new
                    {
                        name = name,
                        GPA = GPA
                    }))
                 .WillRespond()
                 .WithStatus(HttpStatusCode.NotFound);

            await _pact.VerifyAsync(async ctx =>
            {
                var client = new StudentApiClient(ctx.MockServerUri.ToString());
                var response = await client.PatchStudentRestResponse(sId, new StudentPatchDto() { Name = name, GPA = GPA });

                // Client Assertions
                response.Data.Should().BeNull();
                response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            });
        }

        //GetStudents
        [Test]
        public async Task GetStudents_WhenCalled_ReturnsAllStudents()
        {
            _pact.UponReceiving($"a request to get all students")
                    .Given("some students exist")
                    .WithRequest(HttpMethod.Get, $"/students/")
                 .WillRespond()
                 .WithStatus(HttpStatusCode.OK)
                 .WithHeader("Content-Type", "application/json; charset=utf-8")
                 .WithJsonBody(new List<dynamic>()
                 {
                     new
                     {
                        sId = Match.Equality(1),
                        name = Match.Equality("Test student 1"),
                        GPA = Match.Equality(3.99)
                     },
                     new
                     {
                        sId = Match.Equality(2),
                        name = Match.Equality("Test student 2"),
                        GPA = Match.Equality(3.89)
                     },
                     new
                     {
                        sId = Match.Equality(3),
                        name = Match.Equality("Test student 3"),
                        GPA = Match.Equality(3.79)
                     },
                 });

            await _pact.VerifyAsync(async ctx =>
            {
                var client = new StudentApiClient(ctx.MockServerUri.ToString());
                var students = await client.GetStudents();

                // Client Assertions
                students.Should().NotBeNull().And.HaveCountGreaterThan(0);

                foreach( var student in students )
                {
                    student.SId.Should().NotBeNull().And.BeGreaterThan(0);
                    student.Name.Should().NotBeNullOrWhiteSpace();
                    student.GPA.Should().NotBeNull().And.BeGreaterThan(3.7m);
                }
            });
        }

        [Test]
        public async Task GetStudents_WhenNoStudentsExist_Returns404()
        {
            _pact.UponReceiving($"a request to get all students")
                    .Given("no students exist")
                    .WithRequest(HttpMethod.Get, $"/students/")
                 .WillRespond()
                 .WithStatus(HttpStatusCode.NotFound);

            await _pact.VerifyAsync(async ctx =>
            {
                var client = new StudentApiClient(ctx.MockServerUri.ToString());
                var students = await client.GetStudents();

                // Client Assertions
                students.Should().BeNull();
            });
        }

    }
}