using FluentAssertions;
using Harri.SchoolDemoAPI.Client;
using Harri.SchoolDemoAPI.Models;
using Harri.SchoolDemoAPI.Models.Dto;
using Harri.SchoolDemoAPI.Models.Enums;
using Harri.SchoolDemoAPI.Tests.Common;
using Harri.SchoolDemoAPI.Tests.Contract.Consumer.Helpers;
using RestSharp;
using System.Net;
using System.Text.Json;

namespace Harri.SchoolDemoAPI.Tests.Contract.Consumer
{
    public class StudentsQueryApiPagedConsumerTests : ConsumerTestBase
    {
        // Pagination
        private static IEnumerable<TestCaseData> GetValidPagedQueryTestCases()
        {
            yield return new TestCaseData(2, 10, 2, 10, "Test Case 1");
            yield return new TestCaseData(null, null, 1, 10, "Test Case 2");
            yield return new TestCaseData(2, 2, 2, 2, "Test Case 3");
            yield return new TestCaseData(2, null, 2, 10, "Test Case 4");
            yield return new TestCaseData(null, 100, 1, 100, "Test Case 5");
        }
        
        [TestCaseSource(nameof(GetValidPagedQueryTestCases))]
        public async Task QueryStudents_WhenCalled_WithPageAndPageSize_ReturnsMatchingStudents(int? page, int? pageSize, int expectedPage, int expectedPageSize, string testCase)
        {
            var pactBuilder = _pact.UponReceiving($"a valid request to query students with page {page} and page size {pageSize}, {testCase}")
                    .Given("some students exist for querying across multiple pages", new GetStudentsQueryDto() { GPAQueryDto = new(), Page = expectedPage, PageSize = expectedPageSize })
                    .WithRequest(HttpMethod.Get, $"/students/")
                    .SetQueryStringParameters(page: page, pageSize: pageSize)
                 .WillRespond()
                 .WithStatus(HttpStatusCode.OK)
                 .WithHeader("Content-Type", "application/json; charset=utf-8")
                 .WithJsonBody(MockStudentTestFixture.ExpectedStudentsAcrossMultiplePagesJsonBody);

            await _pact.VerifyAsync(async ctx =>
            {
                var client = new StudentApiClient(ctx.MockServerUri.ToString());
                var response = await client.GetStudents(page: page, pageSize: pageSize);

                response.Should().NotBeNull();
                response.Should().BeEquivalentTo(MockStudentTestFixture.MockPagedListAcrossMultiplePages);
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
                    //.Given("some students exist for querying", new GetStudentsQueryDto() { GPAQueryDto = new(), Page = 2, PageSize = 10 })
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

                var response = await client.ExecuteGetAsync<PagedList<StudentDto>?>(request);

                // Client Assertions
                response.Data.Should().BeNull();
                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            });
        }

        //TODO clean this up
        [Test]
        public async Task QueryStudents_WhenCalled_WithBadPagedQuery2_ReturnsBadRequest()
        {
            string? page = "2";
            string? pageSize = "3";

            var pactBuilder = _pact.UponReceiving($"a paged request to query students with out of bounds page {page} and page size {pageSize}")
                    .Given("some students exist for querying", new GetStudentsQueryDto() { GPAQueryDto = new(), Page = 2, PageSize = 3 })
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

                var response = await client.ExecuteGetAsync<PagedList<StudentDto>?>(request);

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
            var page = 1;
            var pageSize = 3;
            var pactBuilder = _pact.UponReceiving($"a valid request to query students with all parameters set")
                    .Given("some students exist for querying", new GetStudentsQueryDto() { SId = sId, Name = name, GPAQueryDto = gpaQuery, OrderBy = orderBy, SortColumn = sortColumn, Page = page, PageSize = pageSize })
                    .WithRequest(HttpMethod.Get, $"/students/")
                    .SetQueryStringParameters(sId, name, gpaQuery, orderBy.ToString(), sortColumn, page, pageSize)
                 .WillRespond()
                 .WithStatus(HttpStatusCode.OK)
                 .WithHeader("Content-Type", "application/json; charset=utf-8")
                 .WithJsonBody(MockStudentTestFixture.ExpectedPagedStudentsJsonBody);

            await _pact.VerifyAsync(async ctx =>
            {
                var client = new StudentApiClient(ctx.MockServerUri.ToString());
                var students = await client.GetStudents(sId, name, gpaQuery, orderBy, sortColumn, page, pageSize);
                StudentsQueryApiTestHelper.AssertStudentsResponseIsCorrect(students);
            });
        }
    }
}
