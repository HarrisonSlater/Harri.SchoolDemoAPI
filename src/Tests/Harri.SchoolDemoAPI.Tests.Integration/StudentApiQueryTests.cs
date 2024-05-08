using FluentAssertions;
using Harri.SchoolDemoApi.Client;
using Harri.SchoolDemoAPI.Models.Dto;
using System.Net;

namespace Harri.SchoolDemoAPI.Tests.Integration
{
    public class StudentApiQueryTests : IntegrationTestBase
    {
        private static StudentApiClient _client;

        //Test student 1
        private static NewStudentDto _studentToMatchName;
        private static int _studentToMatchNameId;

        //Test student 2
        private static NewStudentDto _studentToMatchGpa;
        private static int _studentToMatchGpaId;

        //Test student 3
        private static NewStudentDto _studentToMatchNameAndGpa;
        private static int _studentToMatchNameAndGpaId;

        private static StudentDto ExpectedStudentToFindMatchingName => new()
        {
            SId = _studentToMatchNameId,
            Name = _studentToMatchName.Name,
            GPA = _studentToMatchName.GPA
        };

        private static StudentDto ExpectedStudentToFindMatchingGpa => new()
        {
            SId = _studentToMatchGpaId,
            Name = _studentToMatchGpa.Name,
            GPA = _studentToMatchGpa.GPA
        };
        
        private static StudentDto ExpectedStudentToFindMatchingNameAndGpa => new()
        {
            SId = _studentToMatchNameAndGpaId,
            Name = _studentToMatchNameAndGpa.Name,
            GPA = _studentToMatchNameAndGpa.GPA
        };


        [OneTimeSetUp]
        public static async Task SetUp()
        {
            _client = new StudentApiClient(HostedProvider.ServerUri.AbsoluteUri);

            _studentToMatchName = new NewStudentDto() { Name = "Johnnny 'The Integrator' TestShoes" };
            _studentToMatchNameId = (await _client.AddStudent(_studentToMatchName)).Value;

            _studentToMatchGpa = new NewStudentDto() { Name = "Garry Patrick Anderson", GPA = 2.92m};
            _studentToMatchGpaId = (await _client.AddStudent(_studentToMatchGpa)).Value;

            _studentToMatchNameAndGpa = new NewStudentDto() { Name = Guid.NewGuid().ToString(), GPA = 3.01m };
            _studentToMatchNameAndGpaId = (await _client.AddStudent(_studentToMatchNameAndGpa)).Value;

        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            await CleanUpTestStudent(_studentToMatchNameId);
            await CleanUpTestStudent(_studentToMatchGpaId);
            await CleanUpTestStudent(_studentToMatchNameAndGpaId);
        }

        [Test]
        public async Task QueryStudents_ByName_ShouldReturnBadRequest()
        {
            var response = await _client.QueryStudentsRestResponse(" \t\n  ", null);
            response.Data.Should().BeNull();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task QueryStudents_ShouldReturnBadRequest()
        {
            var response = await _client.QueryStudentsRestResponse(null, null);
            response.Data.Should().BeNull();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task QueryStudents_ByGPA_ShouldReturnBadRequest()
        {
            var response = await _client.QueryStudentsRestResponse(null, new GPAQueryDto() { GPA = new() { Eq = 2, Gt = 2} });
            response.Data.Should().BeNull();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        //TODO cases for names with special characters and unicode symbols
        [TestCase("Johnnny")]
        [TestCase("johnnny")]
        [TestCase("'The Integrator'")]
        [TestCase("'the integrator'")]
        [TestCase("TestShoes")]
        [TestCase("testshoes")]
        [TestCase("Johnnny 'The Integrator' TestShoes")]
        [TestCase("johnnny 'the integrator' testShoes")]
        [TestCase("JOHNNNY 'THE INTEGRATOR' TESTSHOES")]
        public async Task QueryStudents_ShouldMatch_ByName(string name)
        {
            // Arrange
            var expectedStudentToFind = ExpectedStudentToFindMatchingName;

            // Act
            var response = await _client.QueryStudentsRestResponse(name, null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            response.Data.Should().NotBeNull().And.HaveCountGreaterThan(0);
            response.Data.Should().ContainEquivalentOf(expectedStudentToFind);
        }

        [Test]
        public async Task QueryStudents_ShouldNotMatch_ByName()
        {
            // Arrange
            var searchName = Guid.NewGuid().ToString();

            // Act
            var response = await _client.QueryStudentsRestResponse(searchName, null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            response.Data.Should().BeNull();
        }

        private static IEnumerable<TestCaseData> MatchingGPAOnlyTestCases()
        {
            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Eq = 2.92m } });
            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Lt = 2.93m } });
            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Gt = 2.91m } });
            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Gt = 2.91m, Lt = 2.93m } });
        }

        [TestCaseSource(nameof(MatchingGPAOnlyTestCases))]
        public async Task QueryStudents_ShouldMatch_OnGPA(GPAQueryDto gpaQueryDto)
        {
            // Arrange
            var expectedStudentToFind = ExpectedStudentToFindMatchingGpa;

            // Act
            var response = await _client.QueryStudentsRestResponse(null, gpaQueryDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            response.Data.Should().NotBeNull().And.HaveCountGreaterThan(0);
            response.Data.Should().ContainEquivalentOf(expectedStudentToFind);
        }

        private static IEnumerable<TestCaseData> NotMatchingGPAOnlyTestCases()
        {
            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Gt = 2.93m, Lt = 2.91m } });
            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Eq = 2.93m } });
        }

        [TestCaseSource(nameof(NotMatchingGPAOnlyTestCases))]
        public async Task QueryStudents_ShouldNotMatch_OnGPA(GPAQueryDto gpaQueryDto)
        {
            // Arrange
            var expectedStudentToFind = ExpectedStudentToFindMatchingGpa;

            // Act
            var response = await _client.QueryStudentsRestResponse(null, gpaQueryDto);

            // Assert
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                response.Data.Should().NotBeNull().And.HaveCountGreaterThan(0);
                response.Data.Should().NotContainEquivalentOf(expectedStudentToFind);
            }
        }

        private static IEnumerable<TestCaseData> MatchingGPAAndNameTestCases()
        {
            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Eq = 3.01m } });
            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Eq = 3.01m } });
            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Lt = 3.02m } });
            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Gt = 3.00m } });
            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Gt = 3.00m, Lt = 3.02m } });
        }

        [TestCaseSource(nameof(MatchingGPAAndNameTestCases))]
        public async Task QueryStudents_ShouldMatch_OnNameAndGPA(GPAQueryDto gpaQueryDto)
        {
            // Arrange
            var expectedStudentToFind = ExpectedStudentToFindMatchingNameAndGpa;
            var name = expectedStudentToFind.Name;

            // Act
            var response = await _client.QueryStudentsRestResponse(name, gpaQueryDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            response.Data.Should().NotBeNull().And.ContainSingle();
            response.Data!.Single().Should().BeEquivalentTo(expectedStudentToFind);
        }

        private static IEnumerable<TestCaseData> NotMatchingGPAAndNameTestCases()
        {
            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Eq = 3 } });
            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Lt = 3.01m } });
            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Gt = 3.01m } });
            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Gt = 3.01m, Lt = 3.01m } });
        }

        [TestCaseSource(nameof(NotMatchingGPAAndNameTestCases))]
        public async Task QueryStudents_ShouldNotMatch_OnNameAndGPA(GPAQueryDto gpaQueryDto)
        {
            // Arrange
            var expectedStudentToFind = ExpectedStudentToFindMatchingNameAndGpa;
            var name = expectedStudentToFind.Name;

            // Act
            var response = await _client.QueryStudentsRestResponse(name, gpaQueryDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            response.Data.Should().BeNull();
        }

        private async Task CleanUpTestStudent(int sId)
        {
            try
            {
                await _client.DeleteStudent(sId);
            }
            catch { }
        }
    }
}
