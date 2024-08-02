using FluentAssertions;
using Harri.SchoolDemoAPI.Client;
using Harri.SchoolDemoAPI.Models.Dto;
using System.Net;

namespace Harri.SchoolDemoAPI.Tests.E2E
{
    public class StudentApiQueryTests : E2ETestBase
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

        //Test student 4
        private static NewStudentDto _studentToMatchNameSpecial;
        private static int _studentToMatchNameSpecialId;

        //Test student 5
        private static NewStudentDto _studentToMatchNameUnicode;
        private static int _studentToMatchNameUnicodeId;

        private static StudentDto ExpectedStudentToFindMatchingName => GetStudentDtoFor(_studentToMatchNameId, _studentToMatchName);

        private static StudentDto ExpectedStudentToFindMatchingGpa => GetStudentDtoFor(_studentToMatchGpaId, _studentToMatchGpa);
        
        private static StudentDto ExpectedStudentToFindMatchingNameAndGpa => GetStudentDtoFor(_studentToMatchNameAndGpaId, _studentToMatchNameAndGpa);

        private static StudentDto ExpectedStudentToFindMatchingNameSpecial => GetStudentDtoFor(_studentToMatchNameSpecialId, _studentToMatchNameSpecial);

        private static StudentDto ExpectedStudentToFindMatchingNameUnicode => GetStudentDtoFor(_studentToMatchNameUnicodeId, _studentToMatchNameUnicode);

        private static StudentDto GetStudentDtoFor(int id, NewStudentDto newStudent)
        {
            return new StudentDto()
            {
                SId = id,
                Name = newStudent.Name,
                GPA = newStudent.GPA
            };
        }

        [OneTimeSetUp]
        public static async Task SetUp()
        {
            if (APIUrlToTest is null) throw new ArgumentException("APIUrlToTest from appsettings.json cannot be null");

            _client = new StudentApiClient(APIUrlToTest);

            _studentToMatchName = new NewStudentDto() { Name = "Johnnny 'The Integrator' TestShoes" };
            _studentToMatchNameId = (await _client.AddStudent(_studentToMatchName)).Value;

            _studentToMatchGpa = new NewStudentDto() { Name = "Garry Patrick Anderson", GPA = 2.92m};
            _studentToMatchGpaId = (await _client.AddStudent(_studentToMatchGpa)).Value;

            _studentToMatchNameAndGpa = new NewStudentDto() { Name = Guid.NewGuid().ToString(), GPA = 3.01m };
            _studentToMatchNameAndGpaId = (await _client.AddStudent(_studentToMatchNameAndGpa)).Value;

            _studentToMatchNameSpecial = new NewStudentDto() { Name = "Johnnny I. Test-Shoes (123456789)" };
            _studentToMatchNameSpecialId = (await _client.AddStudent(_studentToMatchNameSpecial)).Value;

            _studentToMatchNameUnicode = new NewStudentDto() { Name = "Jöhnnny Äpfelbücher" };
            _studentToMatchNameUnicodeId = (await _client.AddStudent(_studentToMatchNameUnicode)).Value;
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            await CleanUpTestStudent(_studentToMatchNameId);
            await CleanUpTestStudent(_studentToMatchGpaId);
            await CleanUpTestStudent(_studentToMatchNameAndGpaId);
            await CleanUpTestStudent(_studentToMatchNameSpecialId);
            await CleanUpTestStudent(_studentToMatchNameUnicodeId);
        }

        private static IEnumerable<TestCaseData> BadRequestTestCases()
        {
            yield return new TestCaseData(null, new GPAQueryDto() { GPA = new() { Eq = 2, Gt = 2 } });
            yield return new TestCaseData(null, new GPAQueryDto() { GPA = new() { Eq = 2, Gt = 2, IsNull = true } });
        }

        [TestCaseSource(nameof(BadRequestTestCases))]
        public async Task QueryStudents_ByName_ShouldReturnBadRequest(string? name, GPAQueryDto? gpaQuery)
        {
            var response = await _client.GetStudentsRestResponse(name, gpaQuery);
            response.Data.Should().BeNull();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task QueryStudents_ShouldNotMatch_OnName()
        {
            // Arrange
            var searchName = Guid.NewGuid().ToString();

            // Act
            var response = await _client.GetStudentsRestResponse(searchName, null);

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
            var response = await _client.GetStudentsRestResponse(null, gpaQueryDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            response.Data.Should().NotBeNull().And.HaveCountGreaterThan(0);
            response.Data.Should().ContainEquivalentOf(expectedStudentToFind);
            response.Data.Should().AllSatisfy(s => s.GPA.Should().NotBeNull());
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
            var response = await _client.GetStudentsRestResponse(null, gpaQueryDto);

            // Assert
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                response.Data.Should().NotBeNull().And.HaveCountGreaterThan(0);
                response.Data.Should().NotContainEquivalentOf(expectedStudentToFind);
                response.Data.Should().AllSatisfy(s => s.GPA.Should().NotBeNull());
            }
        }

        private static IEnumerable<TestCaseData> MatchingGPAAndNameTestCases()
        {
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
            var response = await _client.GetStudentsRestResponse(name, gpaQueryDto);

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
            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { IsNull = true } });
        }

        [TestCaseSource(nameof(NotMatchingGPAAndNameTestCases))]
        public async Task QueryStudents_ShouldNotMatch_OnNameAndGPA(GPAQueryDto gpaQueryDto)
        {
            // Arrange
            var expectedStudentToFind = ExpectedStudentToFindMatchingNameAndGpa;
            var name = expectedStudentToFind.Name;

            // Act
            var response = await _client.GetStudentsRestResponse(name, gpaQueryDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            response.Data.Should().BeNull();
        }

        private static IEnumerable<TestCaseData> MatchingNullGPATestCases()
        {
            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { IsNull = true } });
        }

        [TestCaseSource(nameof(MatchingNullGPATestCases))]
        public async Task QueryStudents_ShouldMatch_OnNullGpa_WhenIsNull_True(GPAQueryDto gpaQueryDto)
        {
            // Arrange
            var expectedStudentToFind = ExpectedStudentToFindMatchingName;

            // Act
            var response = await _client.GetStudentsRestResponse(ExpectedStudentToFindMatchingName.Name, gpaQueryDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            response.Data.Should().NotBeNull().And.HaveCountGreaterThan(0);
            response.Data.Should().ContainEquivalentOf(expectedStudentToFind);
            response.Data.Should().AllSatisfy(s => s.GPA.Should().BeNull());
        }

        [Test]
        public async Task QueryStudents_ShouldNotMatch_OnNullGpa_WhenIsNull_False()
        {
            // Arrange
            var expectedStudentToFind = ExpectedStudentToFindMatchingName;

            // Act
            var response = await _client.GetStudentsRestResponse(ExpectedStudentToFindMatchingName.Name, new GPAQueryDto() { GPA = new() { IsNull = false } });

            // Assert
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                response.Data.Should().NotBeNull().And.HaveCountGreaterThan(0);
                response.Data.Should().NotContainEquivalentOf(expectedStudentToFind);
                response.Data.Should().AllSatisfy(s => s.GPA.Should().NotBeNull());
            }
            else
            {
                response.Data.Should().BeNull();
            }
        }

        [TestCase(false)]
        [TestCase(null)]
        public async Task QueryStudents_ShouldMatch_OnGpa_WhenIsNull_False_OrNull(bool? isNull)
        {
            // Arrange
            var expectedStudentToFind = ExpectedStudentToFindMatchingNameAndGpa;

            // Act
            var response = await _client.GetStudentsRestResponse(ExpectedStudentToFindMatchingNameAndGpa.Name, new GPAQueryDto() { GPA = new() { IsNull = isNull } });

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            response.Data.Should().NotBeNull().And.HaveCountGreaterThan(0);
            response.Data.Should().ContainEquivalentOf(expectedStudentToFind);
            response.Data.Should().AllSatisfy(s => s.GPA.Should().NotBeNull());
        }

        [Test]
        public async Task QueryStudents_ShouldNotMatch_OnGpa_WhenIsNull_True()
        {
            // Arrange
            var expectedStudentToFind = ExpectedStudentToFindMatchingNameAndGpa;

            // Act
            var response = await _client.GetStudentsRestResponse(ExpectedStudentToFindMatchingNameAndGpa.Name, new GPAQueryDto() { GPA = new() { IsNull = true } });

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            response.Data.Should().BeNull();
        }

        [TestCase("Johnnny 'The Integrator' TestShoes")]

        public async Task QueryStudents_ShouldMatch_OnName(string name)
            => await QueryStudents_ShouldMatch(name, ExpectedStudentToFindMatchingName);

        [TestCase("Johnnny I. Test-Shoes (123456789)")]
        public async Task QueryStudents_ShouldMatch_OnNameSpecial(string name) 
            => await QueryStudents_ShouldMatch(name, ExpectedStudentToFindMatchingNameSpecial);

        [TestCase("Jöhnnny")]

        public async Task QueryStudents_ShouldMatch_OnNameUnicode(string name) 
            => await QueryStudents_ShouldMatch(name, ExpectedStudentToFindMatchingNameUnicode);

        private async Task QueryStudents_ShouldMatch(string name, StudentDto expectedStudentToFind)
        {
            // Act
            var response = await _client.GetStudentsRestResponse(name, null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            response.Data.Should().NotBeNull().And.HaveCountGreaterThan(0);
            response.Data.Should().ContainEquivalentOf(expectedStudentToFind);
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
