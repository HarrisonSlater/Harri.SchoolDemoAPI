using FluentAssertions;
using Harri.SchoolDemoAPI.Client;
using Harri.SchoolDemoAPI.Models;
using Harri.SchoolDemoAPI.Models.Dto;
using Harri.SchoolDemoAPI.Tests.Contract.Consumer.Helpers;
using PactNet.Matchers;
using RestSharp;
using System.Net;

namespace Harri.SchoolDemoAPI.Tests.Contract.Consumer
{
    public class StudentsQueryApiPagedConsumerTests : ConsumerTestBase
    {
        // Sorting
        private static IEnumerable<TestCaseData> GetValidPagedQueryTestCases()
        {
            yield return new TestCaseData(1, 10, 1, 10, "Test Case 1");
            yield return new TestCaseData(null, null, 1, 10, "Test Case 2");
            yield return new TestCaseData(2, 100, 2, 100, "Test Case 3");
            yield return new TestCaseData(2, null, 2, 10, "Test Case 4");
            yield return new TestCaseData(null, 100, 1, 100, "Test Case 3");
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
                 .WithJsonBody(StudentsQueryApiTestHelper.ExpectedPagedStudentsJsonBody);

            await _pact.VerifyAsync(async ctx =>
            {
                var client = new StudentApiClient(ctx.MockServerUri.ToString());
                var response = await client.GetStudents(page: page, pageSize: pageSize);


                StudentsQueryApiTestHelper.AssertStudentsResponseIsCorrect(response);
            });
        }

        private static IEnumerable<TestCaseData> GetBadPagedQueryTestCases()
        {
            yield return new TestCaseData("-1", "-10", "Test Case 1");
            yield return new TestCaseData("-1", "10", "Test Case 2");
            yield return new TestCaseData("1", "-10", "Test Case 3");
            yield return new TestCaseData("asdf", "10", "Test Case 4");
            yield return new TestCaseData("1", "asdf", "Test Case 5");
            yield return new TestCaseData("0", "10", "Test Case 6");
        }

        [TestCaseSource(nameof(GetBadPagedQueryTestCases))]
        public async Task QueryStudents_WhenCalled_WithBadPagedQuery_ReturnsBadRequest(string? page, string? pageSize, string testCase)
        {
            var pactBuilder = _pact.UponReceiving($"a bad paged request to query students with page {page} and page size {pageSize}, {testCase}")
                    //.Given("some students exist for querying")
                    .WithRequest(HttpMethod.Get, $"/students/")
                    .WithQuery(APIConstants.Query.Page, page)
                    .WithQuery(APIConstants.Query.PageSize, pageSize)
                 .WillRespond()
                 .WithStatus(HttpStatusCode.BadRequest);

            await _pact.VerifyAsync(async ctx =>
            {
                var client = new RestClient(ctx.MockServerUri.ToString());
                var request = new RestRequest("students/");
                request.AddQueryParameter(APIConstants.Query.Page, page);
                request.AddQueryParameter(APIConstants.Query.PageSize, pageSize);

                var response = await client.ExecuteGetAsync<List<StudentDto>?>(request);

                // Client Assertions
                response.Data.Should().BeNull();
                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            });
        }

        //All params test
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
