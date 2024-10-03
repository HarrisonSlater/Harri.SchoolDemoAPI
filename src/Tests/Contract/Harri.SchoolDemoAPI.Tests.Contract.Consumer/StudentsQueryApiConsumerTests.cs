using FluentAssertions;
using Harri.SchoolDemoAPI.Client;
using Harri.SchoolDemoAPI.Models;
using Harri.SchoolDemoAPI.Models.Dto;
using Harri.SchoolDemoAPI.Models.Enums;
using Harri.SchoolDemoAPI.Tests.Contract.Consumer.Helpers;
using PactNet;
using PactNet.Matchers;
using RestSharp;
using System.Net;
using System.Text.Json;

namespace Harri.SchoolDemoAPI.Tests.Contract.Consumer
{
    public class StudentsQueryApiConsumerTests : ConsumerTestBase
    {
        private static IEnumerable<TestCaseData> GetInvalidGPAQueryDtoTestCases()
        {
            // testCase string is needed so the test explorer correctly counts these test cases as separate
            // TODO find a better solution for this
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
                    .SetQueryStringParameters(null, "Test Student", gpaQuery)
                    .WillRespond()
                    .WithStatus(HttpStatusCode.BadRequest);

            await _pact.VerifyAsync(async ctx =>
            {
                var client = new StudentApiClient(ctx.MockServerUri.ToString());
                var response = await client.GetStudentsRestResponse(null, name, gpaQuery);

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
            var gpaQuerySerialized = JsonSerializer.Serialize(gpaQuery);
            var pactBuilder = _pact.UponReceiving($"a valid request to query students by name and GPA: {name}, {gpaQuerySerialized} {testCase}")
                    .Given("some students exist for querying", new GetStudentsQueryDto() { Name = name,  GPAQueryDto = gpaQuery })
                    .WithRequest(HttpMethod.Get, $"/students/")
                    .SetQueryStringParameters(null, name, gpaQuery)
                 .WillRespond()
                 .WithStatus(HttpStatusCode.OK)
                 .WithHeader("Content-Type", "application/json; charset=utf-8")
                 .WithJsonBody(StudentsQueryApiTestHelper.ExpectedPagedStudentsJsonBody);

            await _pact.VerifyAsync(async ctx =>
            {
                var client = new StudentApiClient(ctx.MockServerUri.ToString());
                var students = await client.GetStudents(null, name, gpaQuery);
                StudentsQueryApiTestHelper.AssertStudentsResponseIsCorrect(students);
            });
        }

        [TestCaseSource(nameof(GetValidQueryTestCases))]
        public async Task QueryStudents_WhenCalled_WithSId_ReturnsMatchingStudents(string? name, GPAQueryDto? gpaQuery, string testCase)
        {
            var sId = 1234;
            var gpaQuerySerialized = JsonSerializer.Serialize(gpaQuery);
            var pactBuilder = _pact.UponReceiving($"a valid request to query students by sId, name and GPA: {sId}, {name}, {gpaQuerySerialized} {testCase}")
                    .Given("some students exist for querying", new GetStudentsQueryDto() { SId = sId, Name = name,  GPAQueryDto = gpaQuery })
                    .WithRequest(HttpMethod.Get, $"/students/")
                    .SetQueryStringParameters(sId, name, gpaQuery)
                 .WillRespond()
                 .WithStatus(HttpStatusCode.OK)
                 .WithHeader("Content-Type", "application/json; charset=utf-8")
                 .WithJsonBody(StudentsQueryApiTestHelper.ExpectedPagedStudentsJsonBody);

            await _pact.VerifyAsync(async ctx =>
            {
                var client = new StudentApiClient(ctx.MockServerUri.ToString());
                var students = await client.GetStudents(sId, name, gpaQuery);
                StudentsQueryApiTestHelper.AssertStudentsResponseIsCorrect(students);
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
                    .SetQueryStringParameters(null, name, gpaQuery)
                 .WillRespond()
                 .WithStatus(HttpStatusCode.NotFound);

            await _pact.VerifyAsync(async ctx =>
            {
                var client = new StudentApiClient(ctx.MockServerUri.ToString());
                var response = await client.GetStudentsRestResponse(null, name, gpaQuery);

                // Client Assertions
                response.Data.Should().BeNull();
                response.StatusCode.Should().Be(HttpStatusCode.NotFound); 
            });
        }

        // Sorting
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
                    .Given("some students exist for querying", new GetStudentsQueryDto() { Name = name,  GPAQueryDto = gpaQuery, OrderBy = sortOrder })
                    .WithRequest(HttpMethod.Get, $"/students/")
                    .SetQueryStringParameters(null, name, gpaQuery, sortOrderString)
                 .WillRespond()
                 .WithStatus(HttpStatusCode.OK)
                 .WithHeader("Content-Type", "application/json; charset=utf-8")
                 .WithJsonBody(StudentsQueryApiTestHelper.ExpectedPagedStudentsJsonBody);

            await _pact.VerifyAsync(async ctx =>
            {
                var client = new StudentApiClient(ctx.MockServerUri.ToString());
                var students = await client.GetStudents(null, name, gpaQuery, sortOrder);
                StudentsQueryApiTestHelper.AssertStudentsResponseIsCorrect(students);
            });
        }

        private static IEnumerable<TestCaseData> GetValidOrderedQueryCaseInsensitiveTestCases()
        {
            yield return new TestCaseData(SortOrder.ASC, "asc", "Test Case 1");
            yield return new TestCaseData(SortOrder.ASC, "Asc", "Test Case 2");
            yield return new TestCaseData(SortOrder.DESC, "desc", "Test Case 3");
            yield return new TestCaseData(SortOrder.DESC, "Desc", "Test Case 4");
        }

        [TestCaseSource(nameof(GetValidOrderedQueryCaseInsensitiveTestCases))]
        public async Task QueryStudents_WhenCalled_WithCaseInsensitiveSortOrder_ReturnsMatchingStudents(SortOrder? sortOrder, string? sortOrderString, string testCase)
        {
            var pactBuilder = _pact.UponReceiving($"a valid request to query students with case insensitive sort order {sortOrderString}, {testCase}")
                    .Given("some students exist for querying", new GetStudentsQueryDto() { GPAQueryDto = new GPAQueryDto(), OrderBy = sortOrder })
                    .WithRequest(HttpMethod.Get, $"/students/")
                    .WithQuery(APIConstants.Query.OrderBy, sortOrderString)
                 .WillRespond()
                 .WithStatus(HttpStatusCode.OK)
                 .WithHeader("Content-Type", "application/json; charset=utf-8")
                 .WithJsonBody(StudentsQueryApiTestHelper.ExpectedPagedStudentsJsonBody);

            await _pact.VerifyAsync(async ctx =>
            {
                var client = new RestClient(ctx.MockServerUri.ToString());
                var request = new RestRequest("students/");
                request.AddQueryParameter(APIConstants.Query.OrderBy, sortOrderString);

                var response = await client.ExecuteGetAsync<PagedList<StudentDto>?>(request);

                response.Data.Should().NotBeNull();
                StudentsQueryApiTestHelper.AssertStudentsResponseIsCorrect(response.Data);
            });
        }

        private static IEnumerable<TestCaseData> GetBadRequestOrderedQueryTestCases()
        {
            yield return new TestCaseData("asdf", "Test Case 1");
            yield return new TestCaseData("DSC", "Test Case 2");
            yield return new TestCaseData("2", "Test Case 3");
            yield return new TestCaseData("null", "Test Case 4");
        }

        [TestCaseSource(nameof(GetBadRequestOrderedQueryTestCases))]
        public async Task QueryStudents_WhenCalled_WithBadSortOrder_ReturnsBadRequest(string? sortOrderString, string testCase)
        {
            var pactBuilder = _pact.UponReceiving($"a bad request to query students with sort order {sortOrderString}, {testCase}")
                    //.Given("some students exist for querying")
                    .WithRequest(HttpMethod.Get, $"/students/")
                    .WithQuery(APIConstants.Query.OrderBy, sortOrderString)
                 .WillRespond()
                 .WithStatus(HttpStatusCode.BadRequest);

            await _pact.VerifyAsync(async ctx =>
            {
                var client = new RestClient(ctx.MockServerUri.ToString());
                var request = new RestRequest("students/");
                request.AddQueryParameter(APIConstants.Query.OrderBy, sortOrderString);

                var response = await client.ExecuteGetAsync<List<StudentDto>?>(request);

                // Client Assertions
                response.Data.Should().BeNull();
                response.StatusCode.Should().Be(HttpStatusCode.BadRequest); 
            });
        }

        // Sorting by column
        private static IEnumerable<TestCaseData> GetValidOrdered_WithSortColumnQueryTestCases()
        {
            yield return new TestCaseData("sId", "Test Case 1");
            yield return new TestCaseData("name", "Test Case 2");
            yield return new TestCaseData("GPA", "Test Case 3");
            yield return new TestCaseData(null, "Test Case 4");
            yield return new TestCaseData("", "Test Case 5");

            //Case insensitive tests
            yield return new TestCaseData("sid", "Test Case 6");
            yield return new TestCaseData("Name", "Test Case 7");
            yield return new TestCaseData("NAME", "Test Case 8");
            yield return new TestCaseData("Gpa", "Test Case 9");
            yield return new TestCaseData("gpa", "Test Case 10");
        }

        [TestCaseSource(nameof(GetValidOrdered_WithSortColumnQueryTestCases))]
        public async Task QueryStudents_WhenCalled_WithSortColumn_ReturnsMatchingStudents(string? sortColumn, string testCase)
        {
            var name = "Test Student";
            var gpaQuery = new GPAQueryDto() { GPA = new() { Eq = 4 } };
            var gpaQuerySerialized = JsonSerializer.Serialize(gpaQuery);
            var pactBuilder = _pact.UponReceiving($"a valid request to query students with sort column {sortColumn}, {testCase}")
                    .Given("some students exist for querying", new GetStudentsQueryDto() { Name = name, GPAQueryDto = gpaQuery, SortColumn = sortColumn })
                    .WithRequest(HttpMethod.Get, $"/students/")
                    .SetQueryStringParameters(null, name, gpaQuery, sortColumn: sortColumn)
                 .WillRespond()
                 .WithStatus(HttpStatusCode.OK)
                 .WithHeader("Content-Type", "application/json; charset=utf-8")
                 .WithJsonBody(StudentsQueryApiTestHelper.ExpectedPagedStudentsJsonBody);

            await _pact.VerifyAsync(async ctx =>
            {
                var client = new StudentApiClient(ctx.MockServerUri.ToString());
                var students = await client.GetStudents(null, name, gpaQuery, sortColumn: sortColumn);
                StudentsQueryApiTestHelper.AssertStudentsResponseIsCorrect(students);
            });
        }

        private static IEnumerable<TestCaseData> GetBadRequestOrdered_WithSortColumnQueryTestCases()
        {
            yield return new TestCaseData("asdf", "Test Case 1");
            yield return new TestCaseData("DSC", "Test Case 2");
            yield return new TestCaseData("2", "Test Case 3");
        }

        [TestCaseSource(nameof(GetBadRequestOrdered_WithSortColumnQueryTestCases))]
        public async Task QueryStudents_WhenCalled_WithBadSortColumn_ReturnsBadRequest(string? sortColumn, string testCase)
        {
            var pactBuilder = _pact.UponReceiving($"a bad request to query students with sort column {sortColumn}, {testCase}")
                    //.Given("some students exist for querying")
                    .WithRequest(HttpMethod.Get, $"/students/")
                    .WithQuery(APIConstants.Query.SortColumn, sortColumn)
                 .WillRespond()
                 .WithStatus(HttpStatusCode.BadRequest);

            await _pact.VerifyAsync(async ctx =>
            {
                var client = new StudentApiClient(ctx.MockServerUri.ToString());
                var response = await client.GetStudentsRestResponse(sortColumn: sortColumn);

                // Client Assertions
                response.Data.Should().BeNull();
                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            });
        }

        [Test]
        public async Task QueryStudents_WhenCalled_WithAllParametersReturnsMatchingStudents()
        {
            var sId = 1023;
            var name = "Test Student";
            var gpaQuery = new GPAQueryDto() { GPA = new() { Eq = 4 } };
            var gpaQuerySerialized = JsonSerializer.Serialize(gpaQuery);
            var orderBy = SortOrder.ASC;
            var sortColumn = "GPA";
            var pactBuilder = _pact.UponReceiving($"a valid request to query students with all parameters set")
                    .Given("some students exist for querying", new GetStudentsQueryDto() { SId = sId, Name = name, GPAQueryDto = gpaQuery, OrderBy = orderBy, SortColumn = sortColumn })
                    .WithRequest(HttpMethod.Get, $"/students/")
                    .SetQueryStringParameters(sId, name, gpaQuery, orderBy.ToString(), sortColumn: sortColumn)
                 .WillRespond()
                 .WithStatus(HttpStatusCode.OK)
                 .WithHeader("Content-Type", "application/json; charset=utf-8")
                 .WithJsonBody(StudentsQueryApiTestHelper.ExpectedPagedStudentsJsonBody);

            await _pact.VerifyAsync(async ctx =>
            {
                var client = new StudentApiClient(ctx.MockServerUri.ToString());
                var students = await client.GetStudents(sId, name, gpaQuery, orderBy, sortColumn);
                StudentsQueryApiTestHelper.AssertStudentsResponseIsCorrect(students);
            });
        }
    }

    public static class PactTestExtension
    {
        public static IRequestBuilderV4 SetQueryStringParameters(this IRequestBuilderV4 pactBuilder, int? sId = null, string? name = null, GPAQueryDto? gpaQuery = null,
            string? sortOrder = null, string? sortColumn = null, int? page = null, int? pageSize = null)
        {
            if (name is not null)
            {
                pactBuilder.WithQuery("name", name);
            }
            if (sId is not null)
            {
                pactBuilder.WithQuery("sId", sId.Value.ToString());
            }

            if (gpaQuery?.GPA is not null)
            {
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
            }
            if(sortOrder is not null)
            {
                pactBuilder.WithQuery(APIConstants.Query.OrderBy, sortOrder);
            }
            if(sortColumn is not null)
            {
                pactBuilder.WithQuery(APIConstants.Query.SortColumn, sortColumn);
            }
            if(page is not null)
            {
                pactBuilder.WithQuery(APIConstants.Query.Page, page.ToString());
            }
            if(pageSize is not null)
            {
                pactBuilder.WithQuery(APIConstants.Query.PageSize, pageSize.ToString());
            }
            return pactBuilder;
        }
    }
}
