using FluentAssertions;
using Harri.SchoolDemoAPI.Models.Dto;
using Harri.SchoolDemoAPI.Models.Enums;
using Harri.SchoolDemoAPI.Repository;
using Harri.SchoolDemoAPI.Tests.Integration.TestBase;

namespace Harri.SchoolDemoAPI.Tests.Integration
{
    public class StudentRepositoryQueryTests : StudentRepositoryTestBase
    {
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


        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _studentToMatchName = new NewStudentDto() { Name = "Johnnny 'The Integrator' TestShoes" };
            _studentToMatchNameId = await _studentRepository.AddStudent(_studentToMatchName);

            _studentToMatchGpa = new NewStudentDto() { Name = "Garry Patrick Anderson", GPA = 2.92m };
            _studentToMatchGpaId = await _studentRepository.AddStudent(_studentToMatchGpa);

            _studentToMatchNameAndGpa = new NewStudentDto() { Name = Guid.NewGuid().ToString(), GPA = 3.01m };
            _studentToMatchNameAndGpaId = await _studentRepository.AddStudent(_studentToMatchNameAndGpa);

            _studentToMatchNameSpecial = new NewStudentDto() { Name = "Johnnny I. Test-Shoes (123456789)" };
            _studentToMatchNameSpecialId = await _studentRepository.AddStudent(_studentToMatchNameSpecial);

            _studentToMatchNameUnicode = new NewStudentDto() { Name = "Jöhnnny Äpfelbücher" };
            _studentToMatchNameUnicodeId = await _studentRepository.AddStudent(_studentToMatchNameUnicode);
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

        [TestCase("Johnnny")]
        [TestCase("johnnny")]
        [TestCase("'The Integrator'")]
        [TestCase("'the integrator'")]
        [TestCase("TestShoes")]
        [TestCase("testshoes")]
        [TestCase("johnnny 'the integrator' testShoes")]
        [TestCase("JOHNNNY 'THE INTEGRATOR' TESTSHOES")]
        [TestCase("Johnnny 'The Integrator' TestShoes")]
        public async Task GetStudents_ShouldMatch_OnName(string name)
            => await GetStudents_ShouldMatch(name, ExpectedStudentToFindMatchingName);

        [TestCase("Johnnny")]
        [TestCase("johnnny")]
        [TestCase("I.")]
        [TestCase("Test-Shoes")]
        [TestCase("Test")]
        [TestCase("Shoes")]
        [TestCase("(123456789)")]
        [TestCase("123456789")]
        [TestCase("12345")]
        [TestCase("6789")]
        [TestCase("Johnnny I. Test-Shoes (123456789)")]
        public async Task GetStudents_ShouldMatch_OnNameSpecial(string name)
            => await GetStudents_ShouldMatch(name, ExpectedStudentToFindMatchingNameSpecial);

        [TestCase("Jöhnnny")]
        [TestCase("jöhnnny")]
        [TestCase("Äpfel")]
        [TestCase("bücher")]
        [TestCase("Äpfelbücher")]
        [TestCase("äpfelbücher")]
        [TestCase("ö")]
        [TestCase("Jöhnnny Äpfelbücher")]

        public async Task GetStudents_ShouldMatch_OnNameUnicode(string name)
            => await GetStudents_ShouldMatch(name, ExpectedStudentToFindMatchingNameUnicode);

        public async Task GetStudents_ShouldMatch(string name, StudentDto expectedStudentToFind)
        {
            var response = await _studentRepository.GetStudents(new GetStudentsQueryDto() { Name = name });

            response.Should().NotBeNullOrEmpty();
            response.Should().ContainEquivalentOf(expectedStudentToFind);
        }

        [Test]
        public async Task GetStudents_ShouldNotMatch_OnName()
        {
            // Arrange
            var searchName = Guid.NewGuid().ToString();

            // Act
            var response = await _studentRepository.GetStudents(new GetStudentsQueryDto() { Name = searchName });

            // Assert
            response.Should().BeEmpty();
        }

        private static IEnumerable<TestCaseData> MatchingGPAOnlyTestCases()
        {
            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Eq = 2.92m } });
            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Lt = 2.93m } });
            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Gt = 2.91m } });
            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Gt = 2.91m, Lt = 2.93m } });
        }

        [TestCaseSource(nameof(MatchingGPAOnlyTestCases))]
        public async Task GetStudents_ShouldMatch_OnGPA(GPAQueryDto gpaQueryDto)
        {
            // Arrange
            var expectedStudentToFind = ExpectedStudentToFindMatchingGpa;

            // Act
            var response = await _studentRepository.GetStudents(new GetStudentsQueryDto() { GPAQueryDto = gpaQueryDto});

            // Assert
            response.Should().NotBeNullOrEmpty();
            response.Should().ContainEquivalentOf(expectedStudentToFind);

            response.Should().AllSatisfy(s => s.GPA.Should().NotBeNull());

            AssertInAscendingOrder(response);
        }

        private static IEnumerable<TestCaseData> NotMatchingGPAOnlyTestCases()
        {
            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Gt = 2.93m, Lt = 2.91m } });
            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Eq = 2.93m } });
        }

        [TestCaseSource(nameof(NotMatchingGPAOnlyTestCases))]
        public async Task GetStudents_ShouldNotMatch_OnGPA(GPAQueryDto gpaQueryDto)
        {
            // Arrange
            var expectedStudentToFind = ExpectedStudentToFindMatchingGpa;

            // Act
            var response = await _studentRepository.GetStudents(new GetStudentsQueryDto() { GPAQueryDto = gpaQueryDto});

            // Assert
            response.Should().NotBeNull();

            if (response.Count > 0)
            {
                response.Should().NotContainEquivalentOf(expectedStudentToFind);
                response.Should().AllSatisfy(s => s.GPA.Should().NotBeNull());
                AssertInAscendingOrder(response);
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
        public async Task GetStudents_ShouldMatch_OnNameAndGPA(GPAQueryDto gpaQueryDto)
        {
            // Arrange
            var expectedStudentToFind = ExpectedStudentToFindMatchingNameAndGpa;
            var name = expectedStudentToFind.Name;

            // Act
            var response = await _studentRepository.GetStudents(new GetStudentsQueryDto() { Name = name, GPAQueryDto = gpaQueryDto});

            // Assert
            response.Should().NotBeNullOrEmpty();

            response.Should().ContainSingle().And.ContainEquivalentOf(expectedStudentToFind);
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
        public async Task GetStudents_ShouldNotMatch_OnNameAndGPA(GPAQueryDto gpaQueryDto)
        {
            // Arrange
            var expectedStudentToFind = ExpectedStudentToFindMatchingNameAndGpa;
            var name = expectedStudentToFind.Name;

            // Act
            var response = await _studentRepository.GetStudents(new GetStudentsQueryDto() { Name = name, GPAQueryDto = gpaQueryDto});

            // Assert
            response.Should().BeEmpty();
        }

        private static IEnumerable<TestCaseData> MatchingNullGPATestCases()
        {
            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { IsNull = true } });
        }

        [TestCaseSource(nameof(MatchingNullGPATestCases))]
        public async Task GetStudents_ShouldMatch_OnNullGpa_WhenIsNull_True(GPAQueryDto gpaQueryDto)
        {
            // Arrange
            var expectedStudentToFind = ExpectedStudentToFindMatchingName;

            // Act
            var response = await _studentRepository.GetStudents(new GetStudentsQueryDto() { Name = expectedStudentToFind.Name, GPAQueryDto = gpaQueryDto});

            // Assert
            response.Should().NotBeNullOrEmpty();
            response.Should().ContainEquivalentOf(expectedStudentToFind);
            response.Should().AllSatisfy(s => s.GPA.Should().BeNull());

            AssertInAscendingOrder(response);
        }

        [Test]
        public async Task GetStudents_ShouldNotMatch_OnNullGpa_WhenIsNull_False()
        {
            // Arrange
            var expectedStudentToFind = ExpectedStudentToFindMatchingName;

            // Act
            var response = await _studentRepository.GetStudents(new GetStudentsQueryDto() 
            {
                Name = expectedStudentToFind.Name, 
                GPAQueryDto = new GPAQueryDto() { GPA = new() { IsNull = false } } 
            });

            // Assert
            response.Should().NotBeNull();

            if (response.Count > 0)
            {
                response.Should().NotBeNull().And.HaveCountGreaterThan(0);
                response.Should().NotContainEquivalentOf(expectedStudentToFind);
                response.Should().AllSatisfy(s => s.GPA.Should().NotBeNull());

                AssertInAscendingOrder(response);
            }
        }

        [TestCase(false)]
        [TestCase(null)]
        public async Task GetStudents_ShouldMatch_OnGpa_WhenIsNull_False_OrNull(bool? isNull)
        {
            // Arrange
            var expectedStudentToFind = ExpectedStudentToFindMatchingNameAndGpa;

            // Act
            var response = await _studentRepository.GetStudents(new GetStudentsQueryDto() 
            {
                Name = expectedStudentToFind.Name, 
                GPAQueryDto = new GPAQueryDto() { GPA = new() { IsNull = isNull } } 
            });

            // Assert
            response.Should().NotBeNullOrEmpty();
            response.Should().ContainEquivalentOf(expectedStudentToFind);
            response.Should().AllSatisfy(s => s.GPA.Should().NotBeNull());

            AssertInAscendingOrder(response);
        }

        [Test]
        public async Task GetStudents_ShouldNotMatch_OnGpa_WhenIsNull_True()
        {
            // Arrange
            // Act

            var response = await _studentRepository.GetStudents(new GetStudentsQueryDto() 
            {
                Name = ExpectedStudentToFindMatchingNameAndGpa.Name,
                GPAQueryDto = new GPAQueryDto() { GPA = new() { IsNull = true } } 
            });

            // Assert
            response.Should().BeEmpty();
        }

        // Sorting tests
        public void AssertInAscendingOrder(List<StudentDto> response)
        {
            var ids = response.Select(x => x.SId).ToList();
            ids.Count.Should().BeGreaterThan(0);
            ids.Should().BeInAscendingOrder();
        }

        [TestCase(null)]
        [TestCase(SortOrder.ASC)]
        public async Task GetStudents_ShouldOrderByAscending(SortOrder? orderBy)
        {
            // Act
            var response = await _studentRepository.GetStudents(new GetStudentsQueryDto() { OrderBy = orderBy });

            // Assert
            response.Should().NotBeEmpty();

            var ids = response.Select(x => x.SId).ToList();
            ids.Count.Should().BeGreaterThan(1);
            ids.Should().BeInAscendingOrder();
        }

        [Test]
        public async Task GetStudents_ShouldOrderByDescending()
        {
            // Act
            var response = await _studentRepository.GetStudents(new GetStudentsQueryDto() { OrderBy = SortOrder.DESC });

            // Assert
            response.Should().NotBeEmpty();

            var ids = response.Select(x => x.SId).ToList();
            ids.Count.Should().BeGreaterThan(1);
            ids.Should().BeInDescendingOrder();
        }
        
        [Test]
        public async Task GetStudents_ShouldOrderByAscendingWhenFiltering()
        {
            // Act
            var response = await _studentRepository.GetStudents(new GetStudentsQueryDto() { Name = "Smith", OrderBy = SortOrder.ASC });

            // Assert
            response.Should().NotBeEmpty();

            var ids = response.Select(x => x.SId).ToList();
            ids.Count.Should().BeGreaterThan(1);
            ids.Should().BeInAscendingOrder();
        }

        [Test]
        public async Task GetStudents_ShouldOrderByDescending_WhenFiltering()
        {
            // Act
            var response = await _studentRepository.GetStudents(new GetStudentsQueryDto() { Name = "Smith", OrderBy = SortOrder.DESC });

            // Assert
            response.Should().NotBeEmpty();

            var ids = response.Select(x => x.SId).ToList();
            ids.Count.Should().BeGreaterThan(1);
            ids.Should().BeInDescendingOrder();
        }
    }
}
