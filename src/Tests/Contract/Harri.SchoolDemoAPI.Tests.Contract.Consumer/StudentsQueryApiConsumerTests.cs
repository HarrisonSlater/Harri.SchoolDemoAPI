using FluentAssertions;
using Harri.SchoolDemoApi.Client;
using Harri.SchoolDemoAPI.Models.Dto;
using PactNet;
using PactNet.Matchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Harri.SchoolDemoAPI.Tests.Contract.Consumer
{
    public class StudentsQueryApiConsumerTests : ConsumerTestBase
    {
        [Test]
        public async Task QueryStudents_WhenCalledWithoutArguments_Returns400()
        {
            _pact.UponReceiving($"a request to query students")
                    .WithRequest(HttpMethod.Get, $"/students/query")
                    .WillRespond()
                    .WithStatus(HttpStatusCode.BadRequest);

            await _pact.VerifyAsync(async ctx =>
            {
                var client = new StudentApiClient(ctx.MockServerUri.ToString());
                var response = await client.QueryStudentsRestResponse();

                // Client Assertions
                response.Data.Should().BeNull();
                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            });
        }
        private static IEnumerable<TestCaseData> GetInvalidGPAQueryDtoTestCases()
        {
            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Eq = 4, Gt = 4 } });
        }

        [TestCaseSource(nameof(GetInvalidGPAQueryDtoTestCases))]
        public async Task QueryStudents_WhenCalledWithoutInvalidGPAQuery_Returns400(GPAQueryDto gpaQuery)
        {
            var name = "Test Student";
            _pact.UponReceiving($"a request to query students")
                    .WithRequest(HttpMethod.Get, $"/students/query")
                    .SetQueryStringParameters("Test Student", gpaQuery)
                    .WillRespond()
                    .WithStatus(HttpStatusCode.BadRequest);

            await _pact.VerifyAsync(async ctx =>
            {
                var client = new StudentApiClient(ctx.MockServerUri.ToString());
                var response = await client.QueryStudentsRestResponse(name, gpaQuery);

                // Client Assertions
                response.Data.Should().BeNull();
                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            });
        }

        private static IEnumerable<TestCaseData> GetValidQueryTestCases()
        {
            //yield return new TestCaseData("Test Student", new GPAQueryDto() { GPA = new() { Eq = 4 } });
            //yield return new TestCaseData("Test Student", new GPAQueryDto() { GPA = null });
            //yield return new TestCaseData(null, new GPAQueryDto() { GPA = new() { Eq = 4 } });
            //yield return new TestCaseData("Test Student", new GPAQueryDto() { GPA = new() { Lt = 4, Gt = 2 } });
            //yield return new TestCaseData(null, new GPAQueryDto() { GPA = new() { Lt = 2, Gt = 4 } });
            yield return new TestCaseData(null, new GPAQueryDto() { GPA = new() { IsNull = true } });
        }

        [TestCaseSource(nameof(GetValidQueryTestCases))]
        public async Task QueryStudents_WhenCalled_ReturnsMatchingStudents(string? name, GPAQueryDto? gpaQuery)
        {
            var pactBuilder = _pact.UponReceiving($"a request to query students by name and GPA")
                    .Given("some students exist for querying", new Dictionary<string, string>() {
                        //Passed to provider to asserting on the mocked respository
                        {"name", name ?? "null" },
                        {"gpaQuery", JsonSerializer.Serialize(gpaQuery) },
                    })
                    .WithRequest(HttpMethod.Get, $"/students/query")
                    .SetQueryStringParameters(name, gpaQuery)
                 .WillRespond()
                 .WithStatus(HttpStatusCode.OK)
                 .WithHeader("Content-Type", "application/json; charset=utf-8")
                 .WithJsonBody(new List<object>()
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
                var students = await client.QueryStudents(name, gpaQuery);

                // Client Assertions
                students.Should().NotBeNull().And.HaveCountGreaterThan(0);

                foreach (var student in students)
                {
                    student.SId.Should().NotBeNull().And.BeGreaterThan(0);
                    student.Name.Should().NotBeNullOrWhiteSpace();
                    student.GPA.Should().NotBeNull().And.BeGreaterThan(3.7m);
                }
            });
        }

        [Test]
        public async Task QueryStudents_WhenCalledAndNoStudentsAreFound_ReturnsNotFound()
        {
            var name = "Test Student";
            var gpaQuery = new GPAQueryDto() { GPA = new() { Eq = 4 } };
            var pactBuilder = _pact.UponReceiving($"a request to query students by name and GPA")
                    .Given("no students exist for querying")
                    .WithRequest(HttpMethod.Get, $"/students/query")
                    .SetQueryStringParameters(name, gpaQuery)
                 .WillRespond()
                 .WithStatus(HttpStatusCode.NotFound);

            await _pact.VerifyAsync(async ctx =>
            {
                var client = new StudentApiClient(ctx.MockServerUri.ToString());
                var response = await client.QueryStudentsRestResponse(name, gpaQuery);

                // Client Assertions
                response.Data.Should().BeNull();
                response.StatusCode.Should().Be(HttpStatusCode.NotFound); 
            });
        }
    }

    public static class PactTestExtension
    {
        public static IRequestBuilderV4 SetQueryStringParameters(this IRequestBuilderV4 pactBuilder, string? name, GPAQueryDto? gpaQuery)
        {
            if (name is not null)
            {
                pactBuilder.WithQuery("name", name);
            }

            if (gpaQuery?.GPA is null) return pactBuilder;

            if (gpaQuery.GPA.Lt is not null)
            {
                pactBuilder.WithQuery("GPA.lt", gpaQuery.GPA.Lt.ToString());
            }
            if (gpaQuery.GPA.Gt is not null)
            {
                pactBuilder.WithQuery("GPA.gt", gpaQuery.GPA.Gt.ToString());
            }
            if (gpaQuery.GPA.Eq is not null)
            {
                pactBuilder.WithQuery("GPA.eq", gpaQuery.GPA.Eq.ToString());
            }
            if (gpaQuery.GPA.IsNull)
            {
                pactBuilder.WithQuery("GPA.isNull", "True");
            }
            return pactBuilder;
        }
    }
}
