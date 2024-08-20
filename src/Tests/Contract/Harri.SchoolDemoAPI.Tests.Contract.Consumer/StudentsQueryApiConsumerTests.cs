using FluentAssertions;
using Harri.SchoolDemoAPI.Client;
using Harri.SchoolDemoAPI.Models;
using Harri.SchoolDemoAPI.Models.Dto;
using Harri.SchoolDemoAPI.Models.Enums;
using PactNet;
using PactNet.Matchers;
using System.Net;
using System.Text.Json;

namespace Harri.SchoolDemoAPI.Tests.Contract.Consumer
{
    public class StudentsQueryApiConsumerTests : ConsumerTestBase
    {
        // testCase string is needed so the test explorer correctly counts these test cases as separate
        private static IEnumerable<TestCaseData> GetInvalidGPAQueryDtoTestCases()
        {
            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Eq = 4, Gt = 4 } }, "Test Case 1");
            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Eq = 4, Lt = 4 } }, "Test Case 2");
            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Eq = -1 } }, "Test Case 3");
            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Eq = 0 } }, "Test Case 4");
            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Eq = 4, IsNull = true } }, "Test Case 5");
            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Gt = 4, IsNull = false } }, "Test Case 6");
        }

        [TestCaseSource(nameof(GetInvalidGPAQueryDtoTestCases))]
        public async Task QueryStudents_WhenCalledWithoutInvalidGPAQuery_Returns400(GPAQueryDto gpaQuery, string testCase)
        {
            var name = "Test Student";
            var gpaString = JsonSerializer.Serialize(gpaQuery);
            _pact.UponReceiving($"a bad request to query students: {name}, {gpaString} {testCase}")
                    .WithRequest(HttpMethod.Get, $"/students/")
                    .SetQueryStringParameters("Test Student", gpaQuery)
                    .WillRespond()
                    .WithStatus(HttpStatusCode.BadRequest);

            await _pact.VerifyAsync(async ctx =>
            {
                var client = new StudentApiClient(ctx.MockServerUri.ToString());
                var response = await client.GetStudentsRestResponse(name, gpaQuery);

                // Client Assertions
                response.Data.Should().BeNull();
                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            });
        }

        private static IEnumerable<TestCaseData> GetValidQueryTestCases()
        {
            yield return new TestCaseData("Test Student", new GPAQueryDto() { GPA = new() { Eq = 4 } }, "Test Case 1");
            yield return new TestCaseData("Test Student", new GPAQueryDto() { GPA = null }, "Test Case 2");
            yield return new TestCaseData(null, new GPAQueryDto() { GPA = new() { Eq = 4 } }, "Test Case 3");
            yield return new TestCaseData("Test Student", new GPAQueryDto() { GPA = new() { Lt = 4, Gt = 2 } }, "Test Case 4");
            yield return new TestCaseData(null, new GPAQueryDto() { GPA = new() { Lt = 2, Gt = 4 } }, "Test Case 5");
            yield return new TestCaseData(null, new GPAQueryDto() { GPA = new() { Lt = 0.99m, Gt = 0 } }, "Test Case 6");
            yield return new TestCaseData(null, new GPAQueryDto() { GPA = new() { IsNull = true } }, "Test Case 7");
            yield return new TestCaseData("Test Student", new GPAQueryDto() { GPA = new() { IsNull = true } }, "Test Case 8");
            yield return new TestCaseData(null, new GPAQueryDto() { GPA = new() { IsNull = false } }, "Test Case 9");
            yield return new TestCaseData("Test Student", new GPAQueryDto() { GPA = new() { IsNull = false } }, "Test Case 10");
        }

        [TestCaseSource(nameof(GetValidQueryTestCases))]
        public async Task QueryStudents_WhenCalled_ReturnsMatchingStudents(string? name, GPAQueryDto? gpaQuery, string testCase)
        {
            var nameSerialized = name ?? "null";
            var gpaQuerySerialized = JsonSerializer.Serialize(gpaQuery);
            var pactBuilder = _pact.UponReceiving($"a valid request to query students by name and GPA: {nameSerialized}, {gpaQuerySerialized} {testCase}")
                    .Given("some students exist for querying", new Dictionary<string, string>() {
                        //Passed to provider for asserting on the mocked respository
                        {"name", nameSerialized },
                        {"gpaQuery", gpaQuerySerialized },
                    })
                    .WithRequest(HttpMethod.Get, $"/students/")
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
                var students = await client.GetStudents(name, gpaQuery);

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
                    .WithRequest(HttpMethod.Get, $"/students/")
                    .SetQueryStringParameters(name, gpaQuery)
                 .WillRespond()
                 .WithStatus(HttpStatusCode.NotFound);

            await _pact.VerifyAsync(async ctx =>
            {
                var client = new StudentApiClient(ctx.MockServerUri.ToString());
                var response = await client.GetStudentsRestResponse(name, gpaQuery);

                // Client Assertions
                response.Data.Should().BeNull();
                response.StatusCode.Should().Be(HttpStatusCode.NotFound); 
            });
        }

        // Sorting
        //TODO optimise tests
        private static IEnumerable<TestCaseData> GetValidOrderedQueryTestCases()
        {
            yield return new TestCaseData(SortOrder.ASC, "ASC", "Test Case 1");
            yield return new TestCaseData(SortOrder.DESC,"DESC", "Test Case 2");
            yield return new TestCaseData(null, null, "Test Case 3");
        }
        
        [TestCaseSource(nameof(GetValidOrderedQueryTestCases))]
        public async Task QueryStudents_WhenCalled_WithSortOrder_ReturnsMatchingStudents(SortOrder? sortOrder, string? sortOrderString, string testCase)
        {
            var name = "Test Student";
            var gpaQuery = new GPAQueryDto() { GPA = new() { Eq = 4 } };
            var gpaQuerySerialized = JsonSerializer.Serialize(gpaQuery);
            var pactBuilder = _pact.UponReceiving($"a valid request to query students with sort order {sortOrder}, {testCase}")
                    .Given("some students exist for querying", new Dictionary<string, string>() {
                        //Passed to provider for asserting on the mocked respository
                        {"name", name },
                        {"gpaQuery", gpaQuerySerialized },
                        {"orderBy", sortOrderString ?? "null" }
                    })
                    .WithRequest(HttpMethod.Get, $"/students/")
                    .SetQueryStringParameters(name, gpaQuery, sortOrderString)
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
                var students = await client.GetStudents(name, gpaQuery, sortOrder);

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

    }

    public static class PactTestExtension
    {
        public static IRequestBuilderV4 SetQueryStringParameters(this IRequestBuilderV4 pactBuilder, string? name, GPAQueryDto? gpaQuery, string? sortOrder = null)
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
            if (gpaQuery.GPA.IsNull is not null)
            {
                pactBuilder.WithQuery("GPA.isNull", gpaQuery.GPA.IsNull.ToString());
            }
            if(sortOrder is not null)
            {
                pactBuilder.WithQuery(APIConstants.Query.OrderBy, sortOrder);
            }
            return pactBuilder;
        }
    }
}
