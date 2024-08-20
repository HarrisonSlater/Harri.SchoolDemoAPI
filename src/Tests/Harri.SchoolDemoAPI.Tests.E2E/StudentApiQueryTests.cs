using FluentAssertions;
using Harri.SchoolDemoAPI.Client;
using Harri.SchoolDemoAPI.Models.Dto;
using Harri.SchoolDemoAPI.Tests.E2E.TestBase;
using System.Net;

namespace Harri.SchoolDemoAPI.Tests.E2E
{
    public class StudentApiQueryTests : StudentApiTestBase
    {
        private static NewStudentDto _studentToMatch;
        private static int _studentToMatchId;
        private static string _studentToMatchName;

        static StudentApiQueryTests()
        {
            _studentToMatchName = Guid.NewGuid().ToString();
        }

        private static StudentDto ExpectedStudentToFind => GetStudentDtoFor(_studentToMatchId, _studentToMatch);

        [OneTimeSetUp]
        public static async Task SetUp()
        {
            _studentToMatch = new NewStudentDto() { Name = _studentToMatchName, GPA = 3.91m };
            _studentToMatchId = (await _client.AddStudent(_studentToMatch)).Value;
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            await CleanUpTestStudent(_studentToMatchId);
        }

        [Test]
        public async Task GetStudents_ShouldReturnBadRequest()
        {
            var response = await _client.GetStudentsRestResponse(_studentToMatchName, new GPAQueryDto { GPA = new() { Eq = 2, Gt = 2 } });
            response.Data.Should().BeNull();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        private static IEnumerable<TestCaseData> NotMatchingTestCases()
        {
            var testName = "GetStudents_ShouldNotMatch";
            yield return new TestCaseData(_studentToMatchName, new GPAQueryDto() { GPA = new() { Gt = 3.91m, Lt = 3.91m } }) { TestName = testName};

            yield return new TestCaseData(_studentToMatchName, new GPAQueryDto() { GPA = new() { Eq = 2.93m } }) { TestName = testName};
            yield return new TestCaseData(_studentToMatchName,  new GPAQueryDto() { GPA = new() { IsNull = true } }) { TestName = testName};
            yield return new TestCaseData(Guid.NewGuid().ToString(),  new GPAQueryDto() { }) { TestName = testName};
        }

        [TestCaseSource(nameof(NotMatchingTestCases))]
        public async Task GetStudents_ShouldNotMatch(string? name, GPAQueryDto gpaQueryDto)
        {
            // Arrange
            // Act
            var response = await _client.GetStudentsRestResponse(name, gpaQueryDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Data.Should().BeNull();
        }

        private static IEnumerable<TestCaseData> MatchingTestCases()
        {
            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Eq = 3.91m } });
            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Gt = 3.90m, Lt = 3.92m } });
            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { IsNull = false } });
        }

        [TestCaseSource(nameof(MatchingTestCases))]
        public async Task GetStudents_ShouldMatch_OnGPA(GPAQueryDto gpaQueryDto)
        {
            // Arrange
            // Act
            var response = await _client.GetStudentsRestResponse(_studentToMatchName, gpaQueryDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            response.Data.Should().NotBeNull().And.ContainSingle();
            response.Data!.Single().Should().BeEquivalentTo(ExpectedStudentToFind);
        }
    }
}
