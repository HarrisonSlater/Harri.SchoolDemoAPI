using FluentAssertions;
using Harri.SchoolDemoAPI.Client;
using Harri.SchoolDemoAPI.Models;
using Harri.SchoolDemoAPI.Models.Dto;
using Harri.SchoolDemoAPI.Models.Enums;
using PactNet;
using PactNet.Matchers;
using RestSharp;
using System.Net;
using System.Text.Json;

namespace Harri.SchoolDemoAPI.Tests.Contract.Consumer
{
    public class StudentsQueryApiPagedConsumerTests : ConsumerTestBase
    {
        private static dynamic ExpectedStudentsJsonBody = new List<object>()
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
        };

        private static void AssertStudentsResponseIsCorrect(List<StudentDto>? students)
        {
            // Client Assertions
            students.Should().NotBeNull().And.HaveCountGreaterThan(0);

            foreach (var student in students)
            {
                student.SId.Should().NotBeNull().And.BeGreaterThan(0);
                student.Name.Should().NotBeNullOrWhiteSpace();
                student.GPA.Should().NotBeNull().And.BeGreaterThan(3.7m);
            }
        }

        //private static IEnumerable<TestCaseData> GetValidQueryTestCases()
        //{
        //    yield return new TestCaseData("Test Student", new GPAQueryDto() { GPA = new() { Eq = 4 } }, "Test Case 1");
        //    yield return new TestCaseData("Test Student", new GPAQueryDto() { GPA = null }, "Test Case 2");
        //    yield return new TestCaseData(null, new GPAQueryDto() { GPA = new() { Eq = 4 } }, "Test Case 3");
        //    yield return new TestCaseData("Test Student", new GPAQueryDto() { GPA = new() { Lt = 4, Gt = 2 } }, "Test Case 4");
        //    yield return new TestCaseData(null, new GPAQueryDto() { GPA = new() { Lt = 2, Gt = 4 } }, "Test Case 5");
        //    yield return new TestCaseData(null, new GPAQueryDto() { GPA = new() { Lt = 0.99m, Gt = 0 } }, "Test Case 6");
        //    yield return new TestCaseData(null, new GPAQueryDto() { GPA = new() { IsNull = true } }, "Test Case 7");
        //    yield return new TestCaseData("Test Student", new GPAQueryDto() { GPA = new() { IsNull = true } }, "Test Case 8");
        //    yield return new TestCaseData(null, new GPAQueryDto() { GPA = new() { IsNull = false } }, "Test Case 9");
        //    yield return new TestCaseData("Test Student", new GPAQueryDto() { GPA = new() { IsNull = false } }, "Test Case 10");
        //}

        //[TestCaseSource(nameof(GetValidQueryTestCases))]
        //public async Task QueryStudents_WhenCalled_ReturnsMatchingStudents(string? name, GPAQueryDto? gpaQuery, string testCase)
        //{
        //    var gpaQuerySerialized = JsonSerializer.Serialize(gpaQuery);
        //    var pactBuilder = _pact.UponReceiving($"a valid request to query students by name and GPA: {name}, {gpaQuerySerialized} {testCase}")
        //            .Given("some students exist for querying", new GetStudentsQueryDto() { Name = name,  GPAQueryDto = gpaQuery })
        //            .WithRequest(HttpMethod.Get, $"/students/")
        //            .SetQueryStringParameters(name, gpaQuery)
        //         .WillRespond()
        //         .WithStatus(HttpStatusCode.OK)
        //         .WithHeader("Content-Type", "application/json; charset=utf-8")
        //         .WithJsonBody(ExpectedStudentsJsonBody);

        //    await _pact.VerifyAsync(async ctx =>
        //    {
        //        var client = new StudentApiClient(ctx.MockServerUri.ToString());
        //        var students = await client.GetStudents(name, gpaQuery);

        //        AssertStudentsResponseIsCorrect(students);
        //    });
        //}

        // Sorting
        private static IEnumerable<TestCaseData> GetValidPagedQueryTestCases()
        {
            yield return new TestCaseData(1, 10, 1, 10, "Test Case 1");
            yield return new TestCaseData(null, null, 1, 10, "Test Case 2");
        }
        
        [TestCaseSource(nameof(GetValidPagedQueryTestCases))]
        public async Task QueryStudents_WhenCalled_WithPageAndPageSize_ReturnsMatchingStudents(int? page, int? pageSize, int expectedPage, int expectedPageSize, string testCase)
        {
            var pactBuilder = _pact.UponReceiving($"a valid request to query students with page {page} and page size {pageSize}, {testCase}")
                    .Given("some students exist for querying", new GetStudentsQueryDto() { GPAQueryDto = new(), Page = expectedPage, PageSize = expectedPageSize })
                    .WithRequest(HttpMethod.Get, $"/students/")
                    .SetQueryStringParameters(page: page, pageSize: pageSize)
                 .WillRespond()
                 .WithStatus(HttpStatusCode.OK)
                 .WithHeader("Content-Type", "application/json; charset=utf-8")
                 .WithJsonBody(ExpectedStudentsJsonBody);

            await _pact.VerifyAsync(async ctx =>
            {
                var client = new StudentApiClient(ctx.MockServerUri.ToString());
                var students = await client.GetStudents(page: page, pageSize: pageSize);

                AssertStudentsResponseIsCorrect(students);
                //TODO assert page reponse obj
            });
        }

        //private static IEnumerable<TestCaseData> GetValidOrderedQueryCaseInsensitiveTestCases()
        //{
        //    yield return new TestCaseData(SortOrder.ASC, "asc", "Test Case 1");
        //    yield return new TestCaseData(SortOrder.ASC, "Asc", "Test Case 2");
        //    yield return new TestCaseData(SortOrder.DESC, "desc", "Test Case 3");
        //    yield return new TestCaseData(SortOrder.DESC, "Desc", "Test Case 4");
        //}

        //[TestCaseSource(nameof(GetValidOrderedQueryCaseInsensitiveTestCases))]
        //public async Task QueryStudents_WhenCalled_WithCaseInsensitiveSortOrder_ReturnsMatchingStudents(SortOrder? sortOrder, string? sortOrderString, string testCase)
        //{
        //    var pactBuilder = _pact.UponReceiving($"a valid request to query students with case insensitive sort order {sortOrderString}, {testCase}")
        //            .Given("some students exist for querying", new GetStudentsQueryDto() { GPAQueryDto = new GPAQueryDto(), OrderBy = sortOrder })
        //            .WithRequest(HttpMethod.Get, $"/students/")
        //            .WithQuery(APIConstants.Query.OrderBy, sortOrderString)
        //         .WillRespond()
        //         .WithStatus(HttpStatusCode.OK)
        //         .WithHeader("Content-Type", "application/json; charset=utf-8")
        //         .WithJsonBody(ExpectedStudentsJsonBody);

        //    await _pact.VerifyAsync(async ctx =>
        //    {
        //        var client = new RestClient(ctx.MockServerUri.ToString());
        //        var request = new RestRequest("students/");
        //        request.AddQueryParameter(APIConstants.Query.OrderBy, sortOrderString);

        //        var response = await client.ExecuteGetAsync<List<StudentDto>?>(request);

        //        response.Data.Should().NotBeNull();
        //        AssertStudentsResponseIsCorrect(response.Data);
        //    });
        //}

        //private static IEnumerable<TestCaseData> GetBadRequestOrderedQueryTestCases()
        //{
        //    yield return new TestCaseData("asdf", "Test Case 1");
        //    yield return new TestCaseData("DSC", "Test Case 2");
        //    yield return new TestCaseData("2", "Test Case 3");
        //    yield return new TestCaseData("null", "Test Case 4");
        //}

        //[TestCaseSource(nameof(GetBadRequestOrderedQueryTestCases))]
        //public async Task QueryStudents_WhenCalled_WithBadSortOrder_ReturnsBadRequest(string? sortOrderString, string testCase)
        //{
        //    var pactBuilder = _pact.UponReceiving($"a bad request to query students with sort order {sortOrderString}, {testCase}")
        //            //.Given("some students exist for querying")
        //            .WithRequest(HttpMethod.Get, $"/students/")
        //            .WithQuery(APIConstants.Query.OrderBy, sortOrderString)
        //         .WillRespond()
        //         .WithStatus(HttpStatusCode.BadRequest);

        //    await _pact.VerifyAsync(async ctx =>
        //    {
        //        var client = new RestClient(ctx.MockServerUri.ToString());
        //        var request = new RestRequest("students/");
        //        request.AddQueryParameter(APIConstants.Query.OrderBy, sortOrderString);

        //        var response = await client.ExecuteGetAsync<List<StudentDto>?>(request);

        //        // Client Assertions
        //        response.Data.Should().BeNull();
        //        response.StatusCode.Should().Be(HttpStatusCode.BadRequest); 
        //    });
        //}

        //// Sorting by column
        //private static IEnumerable<TestCaseData> GetValidOrdered_WithSortColumnQueryTestCases()
        //{
        //    yield return new TestCaseData("sId", "Test Case 1");
        //    yield return new TestCaseData("name", "Test Case 2");
        //    yield return new TestCaseData("GPA", "Test Case 3");
        //    yield return new TestCaseData(null, "Test Case 4");
        //    yield return new TestCaseData("", "Test Case 5");

        //    //Case insensitive tests
        //    yield return new TestCaseData("sid", "Test Case 6");
        //    yield return new TestCaseData("Name", "Test Case 7");
        //    yield return new TestCaseData("NAME", "Test Case 8");
        //    yield return new TestCaseData("Gpa", "Test Case 9");
        //    yield return new TestCaseData("gpa", "Test Case 10");
        //}

        //[TestCaseSource(nameof(GetValidOrdered_WithSortColumnQueryTestCases))]
        //public async Task QueryStudents_WhenCalled_WithSortColumn_ReturnsMatchingStudents(string? sortColumn, string testCase)
        //{
        //    var name = "Test Student";
        //    var gpaQuery = new GPAQueryDto() { GPA = new() { Eq = 4 } };
        //    var gpaQuerySerialized = JsonSerializer.Serialize(gpaQuery);
        //    var pactBuilder = _pact.UponReceiving($"a valid request to query students with sort column {sortColumn}, {testCase}")
        //            .Given("some students exist for querying", new GetStudentsQueryDto() { Name = name, GPAQueryDto = gpaQuery, SortColumn = sortColumn })
        //            .WithRequest(HttpMethod.Get, $"/students/")
        //            .SetQueryStringParameters(name, gpaQuery, sortColumn: sortColumn)
        //         .WillRespond()
        //         .WithStatus(HttpStatusCode.OK)
        //         .WithHeader("Content-Type", "application/json; charset=utf-8")
        //         .WithJsonBody(ExpectedStudentsJsonBody);

        //    await _pact.VerifyAsync(async ctx =>
        //    {
        //        var client = new StudentApiClient(ctx.MockServerUri.ToString());
        //        var students = await client.GetStudents(name, gpaQuery, sortColumn: sortColumn);

        //        AssertStudentsResponseIsCorrect(students);
        //    });
        //}

        //private static IEnumerable<TestCaseData> GetBadRequestOrdered_WithSortColumnQueryTestCases()
        //{
        //    yield return new TestCaseData("asdf", "Test Case 1");
        //    yield return new TestCaseData("DSC", "Test Case 2");
        //    yield return new TestCaseData("2", "Test Case 3");
        //}

        //[TestCaseSource(nameof(GetBadRequestOrdered_WithSortColumnQueryTestCases))]
        //public async Task QueryStudents_WhenCalled_WithBadSortColumn_ReturnsBadRequest(string? sortColumn, string testCase)
        //{
        //    var pactBuilder = _pact.UponReceiving($"a bad request to query students with sort column {sortColumn}, {testCase}")
        //            //.Given("some students exist for querying")
        //            .WithRequest(HttpMethod.Get, $"/students/")
        //            .WithQuery(APIConstants.Query.SortColumn, sortColumn)
        //         .WillRespond()
        //         .WithStatus(HttpStatusCode.BadRequest);

        //    await _pact.VerifyAsync(async ctx =>
        //    {
        //        var client = new StudentApiClient(ctx.MockServerUri.ToString());
        //        var response = await client.GetStudentsRestResponse(sortColumn: sortColumn);

        //        // Client Assertions
        //        response.Data.Should().BeNull();
        //        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        //    });
        //}

        //[Test]
        //public async Task QueryStudents_WhenCalled_WithAllParametersReturnsMatchingStudents()
        //{
        //    var name = "Test Student";
        //    var gpaQuery = new GPAQueryDto() { GPA = new() { Eq = 4 } };
        //    var gpaQuerySerialized = JsonSerializer.Serialize(gpaQuery);
        //    var orderBy = SortOrder.ASC;
        //    var sortColumn = "GPA";
        //    var pactBuilder = _pact.UponReceiving($"a valid request to query students with all parameters set")
        //            .Given("some students exist for querying", new GetStudentsQueryDto() { Name = name, GPAQueryDto = gpaQuery, OrderBy = orderBy, SortColumn = sortColumn })
        //            .WithRequest(HttpMethod.Get, $"/students/")
        //            .SetQueryStringParameters(name, gpaQuery, orderBy.ToString(), sortColumn: sortColumn)
        //         .WillRespond()
        //         .WithStatus(HttpStatusCode.OK)
        //         .WithHeader("Content-Type", "application/json; charset=utf-8")
        //         .WithJsonBody(ExpectedStudentsJsonBody);

        //    await _pact.VerifyAsync(async ctx =>
        //    {
        //        var client = new StudentApiClient(ctx.MockServerUri.ToString());
        //        var students = await client.GetStudents(name, gpaQuery, orderBy, sortColumn);

        //        AssertStudentsResponseIsCorrect(students);
        //    });
        //}
    }
}
