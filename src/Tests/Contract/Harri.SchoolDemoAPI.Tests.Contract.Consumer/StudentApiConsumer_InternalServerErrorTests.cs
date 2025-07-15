using FluentAssertions;
using Harri.SchoolDemoAPI.Client;
using System.Net;
using Harri.SchoolDemoAPI.Models.Dto;
using RestSharp;
using Harri.SchoolDemoAPI.Tests.Common;
using Microsoft.Net.Http.Headers;

namespace Harri.SchoolDemoAPI.Tests.Contract.Consumer
{
    public class StudentApiConsumer_InternalServerErrorTests : ConsumerTestBase
    {
        // 500 Server error cases
        private static IEnumerable<TestCaseData> StudentApiClientOperationsTestCases()
        {
            var sId = 123;

            yield return new TestCaseData((IStudentApi c) => c.GetStudent(sId), HttpMethod.Get, $"/students/{sId}", "get a student");
            yield return new TestCaseData((IStudentApi c) => c.GetStudentRestResponse(sId), HttpMethod.Get, $"/students/{sId}", "get a student with rest response");

            yield return new TestCaseData((IStudentApi c) => c.GetStudents(), HttpMethod.Get, $"/students/", "get students");
            yield return new TestCaseData((IStudentApi c) => c.GetStudentsRestResponse(), HttpMethod.Get, $"/students/", "get students with rest response");

            yield return new TestCaseData((IStudentApi c) => c.DeleteStudentRestResponse(sId), HttpMethod.Delete, $"/students/{sId}", "delete a student with rest response");
        }

        [TestCaseSource(nameof(StudentApiClientOperationsTestCases))]
        public async Task StudentApiClientOperations_WhenCalled_AndServiceReturns500_DoNotThrow(Func<IStudentApi, dynamic> clientOperation, 
            HttpMethod operationMethod, string operationRoute, string operationName)
        {
            var pactRequest = _pact.UponReceiving($"a request to {operationName}")
                        .Given("the api returns a 500 internal server error")
                        .WithRequest(operationMethod, operationRoute)
                    .WillRespond()
                    .WithStatus(HttpStatusCode.InternalServerError);

            await VerifyClientOperation(clientOperation);
        }

        private static IEnumerable<TestCaseData> WithBody_StudentApiClientOperationsTestCases()
        {
            var sId = 123;
            var sName = "Test Student";
            var rowVersion = MockStudentTestFixture.MockRowVersion;
            var jsonBody = new { name = "Test Student", GPA = (decimal?)null };
            var patchJsonBody = new { name = "Test Student" };

            yield return new TestCaseData((IStudentApi c) => c.AddStudent(new NewStudentDto() { Name = sName }), HttpMethod.Post, "/students/", "add a student", jsonBody);
            yield return new TestCaseData((IStudentApi c) => c.AddStudentRestResponse(new NewStudentDto() { Name = sName }), HttpMethod.Post, "/students/", "add a student with rest response", jsonBody);
        }

        [TestCaseSource(nameof(WithBody_StudentApiClientOperationsTestCases))]
        public async Task StudentApiClientOperations_WhenCalled_WithBody_AndServiceReturns500_DoNotThrow(Func<IStudentApi, dynamic> clientOperation,
           HttpMethod operationMethod, string operationRoute, string operationName, object jsonBody)
        {
            var pactRequest = _pact.UponReceiving($"a request to {operationName}")
                        .Given("the api returns a 500 internal server error")
                        .WithRequest(operationMethod, operationRoute)
                        .WithHeader("Content-Type", "application/json; charset=utf-8")
                        .WithJsonBody(jsonBody)
                    .WillRespond()
                    .WithStatus(HttpStatusCode.InternalServerError);

            await VerifyClientOperation(clientOperation);
        }

        private static IEnumerable<TestCaseData> WithBodyAndHeader_StudentApiClientOperationsTestCases()
        {
            var sId = 123;
            var sName = "Test Student";
            var rowVersion = MockStudentTestFixture.MockRowVersion;
            var jsonBody = new { name = "Test Student", GPA = (decimal?)null };
            var patchJsonBody = new { name = "Test Student" };

            yield return new TestCaseData((IStudentApi c) => c.PatchStudent(sId, new StudentPatchDto() { Name = sName }, rowVersion), HttpMethod.Patch, $"/students/{sId}", "patch a student", patchJsonBody);
            yield return new TestCaseData((IStudentApi c) => c.PatchStudentRestResponse(sId, new StudentPatchDto() { Name = sName }, rowVersion), HttpMethod.Patch, $"/students/{sId}", "patch a student with rest response", patchJsonBody);

            yield return new TestCaseData((IStudentApi c) => c.UpdateStudent(sId, new UpdateStudentDto() { Name = sName }, rowVersion), HttpMethod.Put, $"/students/{sId}", "update a student", jsonBody);
            yield return new TestCaseData((IStudentApi c) => c.UpdateStudentRestResponse(sId, new UpdateStudentDto() { Name = sName }, rowVersion), HttpMethod.Put, $"/students/{sId}", "update a student with rest response", jsonBody);
        }

        [TestCaseSource(nameof(WithBodyAndHeader_StudentApiClientOperationsTestCases))]
        public async Task StudentApiClientOperations_WhenCalled_WithBodyAndIfMatchHeader_AndServiceReturns500_DoNotThrow(Func<IStudentApi, dynamic> clientOperation,
           HttpMethod operationMethod, string operationRoute, string operationName, object jsonBody)
        {
            var pactRequest = _pact.UponReceiving($"a request to {operationName}")
                        .Given("the api returns a 500 internal server error")
                        .WithRequest(operationMethod, operationRoute)
                        .WithHeader("Content-Type", "application/json; charset=utf-8")
                        .WithHeader(HeaderNames.IfMatch, MockStudentTestFixture.MockRowVersionBase64Encoded) 
                        .WithJsonBody(jsonBody)
                    .WillRespond()
                    .WithStatus(HttpStatusCode.InternalServerError);

            await VerifyClientOperation(clientOperation);
        }

        private static IEnumerable<TestCaseData> QueryString_StudentApiClientOperationsTestCases()
        {
            var sName = "Test Student";

            yield return new TestCaseData((IStudentApi c) => c.GetStudents(null, sName), HttpMethod.Get, $"/students/", "get students with query", ("name", sName));
            yield return new TestCaseData((IStudentApi c) => c.GetStudentsRestResponse(null, sName), HttpMethod.Get, $"/students/", "get students with query rest response", ("name", sName));
        }

        [TestCaseSource(nameof(QueryString_StudentApiClientOperationsTestCases))]
        public async Task QueryString_StudentApiClientOperations_WhenCalled_AndServiceReturns500_DoNotThrow(Func<IStudentApi, dynamic> clientOperation,
            HttpMethod operationMethod, string operationRoute, string operationName, (string, string) queryString)
        {
            _pact.UponReceiving($"a request to {operationName}")
                    .Given("the api returns a 500 internal server error")
                    .WithRequest(operationMethod, operationRoute)
                    .WithQuery(queryString.Item1, queryString.Item2)
                    .WillRespond()
                 .WithStatus(HttpStatusCode.InternalServerError);

            await VerifyClientOperation(clientOperation);
        }

        private static IEnumerable<TestCaseData> BoolResponse_StudentApiClientOperationsTestCases()
        {
            var sId = 123;
            yield return new TestCaseData((IStudentApi c) => c.DeleteStudent(sId), HttpMethod.Delete, $"/students/{sId}", "delete a student");
        }

        //TODO refactor these
        [TestCaseSource(nameof(BoolResponse_StudentApiClientOperationsTestCases))]
        public async Task BoolResponse_StudentApiClientOperations_WhenCalled_AndServiceReturns500_DoNotThrow(Func<IStudentApi, dynamic> clientOperation,
            HttpMethod operationMethod, string operationRoute, string operationName)
        {
            _pact.UponReceiving($"a request to {operationName}")
                    .Given("the api returns a 500 internal server error")
                    .WithRequest(operationMethod, operationRoute)
                    .WillRespond()
                 .WithStatus(HttpStatusCode.InternalServerError);

            await _pact.VerifyAsync(async ctx =>
            {
                var client = new StudentApiClient(ctx.MockServerUri.ToString());
                var responseObject = (object)await clientOperation(client);

                // Client Assertions
                if (responseObject is RestResponse restResponse)
                {
                    restResponse.Content.Should().BeEmpty();
                    restResponse.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
                }
                else
                {
                    var boolResponse = (bool)responseObject;
                    boolResponse.Should().BeFalse();
                }
            });
        }

        private async Task VerifyClientOperation(Func<IStudentApi, dynamic> clientOperation)
        {
            await _pact.VerifyAsync(async ctx =>
            {
                var client = new StudentApiClient(ctx.MockServerUri.ToString());
                var responseObject = (object)await clientOperation(client);

                // Client Assertions
                AssertResponseObject(responseObject);
            });
        }

        private void AssertResponseObject(object? responseObject)
        {
            if (responseObject is RestResponse restResponse)
            {
                restResponse.Content.Should().BeEmpty();
                restResponse.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            }
            else
            {
                responseObject.Should().BeOneOf(null, false);
            }
        }
    }
}