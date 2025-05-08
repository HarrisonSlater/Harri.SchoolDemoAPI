using FluentAssertions;
using Harri.SchoolDemoAPI.Client;
using Harri.SchoolDemoAPI.Models;
using Harri.SchoolDemoAPI.Models.Dto;
using Harri.SchoolDemoAPI.Models.Enums;
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
        public async Task GetStudents_ShouldGetAllStudents()
        {
            // Arrange
            // Act
            var response = await _client.GetStudents(pageSize: 1000);
            var students = response.Items;

            // Assert
            students.Should().NotBeNullOrEmpty().And.HaveCountGreaterThan(900);

            var ids = students!.Select(x => x.SId).ToList();
            ids.Should().BeInAscendingOrder();
        }


        [Test]
        public async Task GetStudents_ShouldGetAllStudents_Descending()
        {
            // Arrange
            // Act
            var response = await _client.GetStudents(pageSize: 1000, orderBy: SortOrder.DESC);
            var students = response.Items;

            // Assert
            students.Should().NotBeNullOrEmpty().And.HaveCountGreaterThan(900);

            var ids = students!.Select(x => x.SId).ToList();
            ids.Should().BeInDescendingOrder();
        }

        [Test]
        public async Task GetStudents_ShouldReturnBadRequest()
        {
            var response = await _client.GetStudentsRestResponse(null, _studentToMatchName, new GPAQueryDto { GPA = new() { Eq = 2, Gt = 2 } });
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
            var response = await _client.GetStudentsRestResponse(null, name, gpaQueryDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
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
            var response = await _client.GetStudentsRestResponse(null, _studentToMatchName, gpaQueryDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            response.Data.Items.Should().NotBeNull().And.ContainSingle();
            response.Data.Items.Single().Should().BeEquivalentTo(ExpectedStudentToFind);
        }

        [Test]
        public async Task GetStudents_ShouldMatch_OnSId()
        {
            // Arrange
            // Act
            var response = await _client.GetStudentsRestResponse(_studentToMatchId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            response.Data.Items.Should().NotBeNull().And.ContainSingle();
            response.Data.Items.Single().Should().BeEquivalentTo(ExpectedStudentToFind);
        }

        // Ordering tests
        [Test]
        public async Task GetStudents_ShouldGetAllStudentsAscending_ByName()
        {
            // Arrange
            // Act
            var response = await _client.GetStudents(null, "Smith", orderBy: SortOrder.ASC, sortColumn: APIConstants.Student.Name);
            var students = response.Items;


            // Assert
            students.Should().NotBeNullOrEmpty().And.HaveCountGreaterThan(1);

            var names = students!.Select(x => x.Name).ToList();
            names.Should().BeInAscendingOrder();
        }

        [Test]
        public async Task GetStudents_ShouldGetAllStudentsDescending_ByName()
        {
            // Arrange
            // Act
            var response = await _client.GetStudents(null, "Anderson", orderBy: SortOrder.DESC, sortColumn: APIConstants.Student.Name);
            var students = response.Items;

            // Assert
            students.Should().NotBeNullOrEmpty().And.HaveCountGreaterThan(1);

            var names = students!.Select(x => x.Name).ToList();
            names.Should().BeInDescendingOrder();
        }

        [Test]
        public async Task GetStudents_ShouldGetAllStudents_BySId()
        {
            // Arrange
            var idToSearch = 1;
            // Act
            var response = await _client.GetStudents(idToSearch);
            var students = response.Items;

            // Assert
            students.Should().NotBeNullOrEmpty().And.HaveCountGreaterThan(1);

            var ids = students!.Select(x => x.SId).ToList();
            ids.Should().BeInAscendingOrder();
            ids.Should().AllSatisfy(id => id.ToString().Should().Contain(idToSearch.ToString()));
        }

        [Test]
        [NonParallelizable]
        public async Task GetStudents_ShouldGetMultiplePages()
        {
            // Arrange
            var page = 1;
            var allStudents = new List<StudentDto>();
            int? totalCount = null;
            PagedList<StudentDto>? response;

            // Act
            do
            {
                response = await _client.GetStudents(name: "Anderson", page: page, orderBy: SortOrder.DESC, sortColumn: APIConstants.Student.Name);

                if (totalCount is null)
                {
                    totalCount = response.TotalCount;
                }

                allStudents.AddRange(response.Items);

                page++;
            } while (response.HasNextPage); // Get all pages

            // Assert
            allStudents.Should().NotBeNullOrEmpty().And.HaveCount(totalCount.Value);

            var names = allStudents!.Select(x => x.Name).ToList();
            names.Should().BeInDescendingOrder();
        }
    }
}
