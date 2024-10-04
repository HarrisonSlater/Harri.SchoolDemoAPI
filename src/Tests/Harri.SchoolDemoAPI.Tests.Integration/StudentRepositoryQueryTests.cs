using FluentAssertions;
using Harri.SchoolDemoAPI.Models;
using Harri.SchoolDemoAPI.Models.Dto;
using Harri.SchoolDemoAPI.Models.Enums;
using Harri.SchoolDemoAPI.Tests.Integration.TestBase;
using NUnit.Framework.Internal;

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

        private GetStudentsQueryDto _studentsQueryDto;

        [SetUp]
        public void SetUp()
        {
            _studentsQueryDto = new GetStudentsQueryDto() { Page = 1, PageSize = 10, OrderBy = SortOrder.ASC, SortColumn = APIConstants.Student.SId };
        }

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
        public static async Task TearDown()
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
            // Arrange
            _studentsQueryDto.Name = name;
            _studentsQueryDto.PageSize = 100;

            // Act
            var response = await _studentRepository.GetStudents(_studentsQueryDto);

            // Assert

            response.Items.Should().NotBeEmpty().And.HaveCountGreaterThanOrEqualTo(1);
            response.Page.Should().Be(1);
            response.PageSize.Should().Be(100);
            response.TotalCount.Should().BeGreaterThanOrEqualTo(1);

            response.Items.Should().ContainEquivalentOf(expectedStudentToFind);
        }

        [Test]
        public async Task GetStudents_ShouldNotMatch_OnName()
        {
            // Arrange
            var searchName = Guid.NewGuid().ToString();
            _studentsQueryDto.Name = searchName;

            // Act
            var response = await _studentRepository.GetStudents(_studentsQueryDto);

            // Assert
            response.ShouldHaveEmptyPaginationData();
        }

        private static IEnumerable<TestCaseData> ShouldMatch_OnSId_TestCases()
        {
            yield return new TestCaseData(ExpectedStudentToFindMatchingName);
            yield return new TestCaseData(ExpectedStudentToFindMatchingGpa);
            yield return new TestCaseData(ExpectedStudentToFindMatchingNameAndGpa);
            yield return new TestCaseData(ExpectedStudentToFindMatchingNameSpecial);
            yield return new TestCaseData(ExpectedStudentToFindMatchingNameUnicode);
        }

        [TestCaseSource(nameof(ShouldMatch_OnSId_TestCases))]
        public async Task GetStudents_ShouldMatch_OnSId(StudentDto expectedStudentToFind)
        {
            // Arrange
            _studentsQueryDto.SId = expectedStudentToFind.SId;

            // Act
            var response = await _studentRepository.GetStudents(_studentsQueryDto);
            var items = response.Items;

            // Assert
            items.Should().NotBeEmpty().And.HaveCount(1);
            response.Page.Should().Be(1);
            response.TotalCount.Should().Be(1);

            items.Should().AllSatisfy(s => s.SId.ToString().Should().Contain(expectedStudentToFind.SId.ToString()));
            items.Should().ContainSingle().And.ContainEquivalentOf(expectedStudentToFind);
            items.ShouldBeAscendingOrderedBySId();
        }

        private static int GetIntPartial(int i, int take) => 
            int.Parse(new string(i.ToString().ToArray().Take(take).ToArray()));

        private static int GetIntPartialFromEnd(int i, int takeLast) => 
            int.Parse(new string(i.ToString().ToArray().TakeLast(takeLast).ToArray()));

        private static IEnumerable<TestCaseData> ShouldMatch_OnSIdPartial_TestCases()
        {
            yield return new TestCaseData(GetIntPartial(_studentToMatchNameId, 1), ExpectedStudentToFindMatchingName);
            yield return new TestCaseData(GetIntPartial(_studentToMatchGpaId, 1), ExpectedStudentToFindMatchingGpa);
            yield return new TestCaseData(GetIntPartial(_studentToMatchNameAndGpaId, 1), ExpectedStudentToFindMatchingNameAndGpa);
            yield return new TestCaseData(GetIntPartial(_studentToMatchNameSpecialId, 1), ExpectedStudentToFindMatchingNameSpecial);
            yield return new TestCaseData(GetIntPartial(_studentToMatchNameUnicodeId, 1), ExpectedStudentToFindMatchingNameUnicode);

            yield return new TestCaseData(GetIntPartial(_studentToMatchNameId, 2), ExpectedStudentToFindMatchingName);
            yield return new TestCaseData(GetIntPartial(_studentToMatchGpaId, 2), ExpectedStudentToFindMatchingGpa);
            yield return new TestCaseData(GetIntPartial(_studentToMatchNameAndGpaId, 2), ExpectedStudentToFindMatchingNameAndGpa);
            yield return new TestCaseData(GetIntPartial(_studentToMatchNameSpecialId, 2), ExpectedStudentToFindMatchingNameSpecial);
            yield return new TestCaseData(GetIntPartial(_studentToMatchNameUnicodeId, 2), ExpectedStudentToFindMatchingNameUnicode);

            yield return new TestCaseData(GetIntPartial(_studentToMatchNameId, 3), ExpectedStudentToFindMatchingName);
            yield return new TestCaseData(GetIntPartial(_studentToMatchGpaId, 3), ExpectedStudentToFindMatchingGpa);
            yield return new TestCaseData(GetIntPartial(_studentToMatchNameAndGpaId, 3), ExpectedStudentToFindMatchingNameAndGpa);
            yield return new TestCaseData(GetIntPartial(_studentToMatchNameSpecialId, 3), ExpectedStudentToFindMatchingNameSpecial);
            yield return new TestCaseData(GetIntPartial(_studentToMatchNameUnicodeId, 3), ExpectedStudentToFindMatchingNameUnicode);

            yield return new TestCaseData(GetIntPartialFromEnd(_studentToMatchNameId, 2), ExpectedStudentToFindMatchingName);
            yield return new TestCaseData(GetIntPartialFromEnd(_studentToMatchGpaId, 2), ExpectedStudentToFindMatchingGpa);
            yield return new TestCaseData(GetIntPartialFromEnd(_studentToMatchNameAndGpaId, 2), ExpectedStudentToFindMatchingNameAndGpa);
            yield return new TestCaseData(GetIntPartialFromEnd(_studentToMatchNameSpecialId, 2), ExpectedStudentToFindMatchingNameSpecial);
            yield return new TestCaseData(GetIntPartialFromEnd(_studentToMatchNameUnicodeId, 2), ExpectedStudentToFindMatchingNameUnicode);

            yield return new TestCaseData(GetIntPartialFromEnd(_studentToMatchNameId, 3), ExpectedStudentToFindMatchingName);
            yield return new TestCaseData(GetIntPartialFromEnd(_studentToMatchGpaId, 3), ExpectedStudentToFindMatchingGpa);
            yield return new TestCaseData(GetIntPartialFromEnd(_studentToMatchNameAndGpaId, 3), ExpectedStudentToFindMatchingNameAndGpa);
            yield return new TestCaseData(GetIntPartialFromEnd(_studentToMatchNameSpecialId, 3), ExpectedStudentToFindMatchingNameSpecial);
            yield return new TestCaseData(GetIntPartialFromEnd(_studentToMatchNameUnicodeId, 3), ExpectedStudentToFindMatchingNameUnicode);
        }

        [TestCaseSource(nameof(ShouldMatch_OnSIdPartial_TestCases))]
        public async Task GetStudents_ShouldMatch_OnSId(int sIdPartial, StudentDto expectedStudentToFind)
        {
            // Arrange
            _studentsQueryDto.SId = sIdPartial;
            _studentsQueryDto.PageSize = 1000;

            // Act
            var response = await _studentRepository.GetStudents(_studentsQueryDto);
            var items = response.Items;

            // Assert
            items.Should().NotBeEmpty().And.HaveCountGreaterThanOrEqualTo(1);
            response.Page.Should().Be(1);
            response.PageSize.Should().Be(1000);
            response.TotalCount.Should().BeGreaterThanOrEqualTo(1);

            items.Should().AllSatisfy(s => s.SId.ToString().Should().Contain(sIdPartial.ToString()));
            items.Should().ContainEquivalentOf(expectedStudentToFind);
            items.ShouldBeAscendingOrderedBySId();
        }

        [TestCase(-1)]
        [TestCase(int.MaxValue)]
        [TestCase(int.MinValue)]
        public async Task GetStudents_ShouldNotMatch_OnSId(int sId)
        {
            // Arrange
            _studentsQueryDto.SId = sId;

            // Act
            var response = await _studentRepository.GetStudents(_studentsQueryDto);

            // Assert
            response.ShouldHaveEmptyPaginationData();
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
            _studentsQueryDto.GPAQueryDto = gpaQueryDto;
            _studentsQueryDto.PageSize = 1000; 

            // Act
            var response = await _studentRepository.GetStudents(_studentsQueryDto);
            var items = response.Items;

            // Assert
            response.Items.Should().NotBeEmpty().And.HaveCountGreaterThanOrEqualTo(1);
            response.Page.Should().Be(1);
            response.PageSize.Should().BeGreaterThanOrEqualTo(1);
            response.TotalCount.Should().BeGreaterThanOrEqualTo(1);

            items.Should().ContainEquivalentOf(expectedStudentToFind);

            items.Should().AllSatisfy(s => s.GPA.Should().NotBeNull());
            items.ShouldBeAscendingOrderedBySId();
        }

        private static IEnumerable<TestCaseData> NotMatchingGPAOnlyTestCases()
        {
            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Gt = 2.93m, Lt = 2.91m } });
            yield return new TestCaseData(new GPAQueryDto() { GPA = new() { Eq = 999.99m } });
        }

        [TestCaseSource(nameof(NotMatchingGPAOnlyTestCases))]
        public async Task GetStudents_ShouldNotMatch_OnGPA(GPAQueryDto gpaQueryDto)
        {
            // Arrange
            var expectedStudentToFind = ExpectedStudentToFindMatchingGpa;
            _studentsQueryDto.GPAQueryDto = gpaQueryDto;

            // Act
            var response = await _studentRepository.GetStudents(_studentsQueryDto);
            var items = response.Items;

            // Assert
            response.Should().NotBeNull();

            if (items.Count > 0)
            {
                items.Should().NotContainEquivalentOf(expectedStudentToFind);
                items.Should().AllSatisfy(s => s.GPA.Should().NotBeNull());
                items.ShouldBeAscendingOrderedBySId();
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

            _studentsQueryDto.Name = name;
            _studentsQueryDto.GPAQueryDto = gpaQueryDto;

            // Act
            var response = await _studentRepository.GetStudents(_studentsQueryDto);

            // Assert
            response.ShouldHaveSingleItemPaginationData();

            response.Items.Should().ContainSingle().And.ContainEquivalentOf(expectedStudentToFind);
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
            _studentsQueryDto.Name = name;
            _studentsQueryDto.GPAQueryDto = gpaQueryDto;

            // Act
            var response = await _studentRepository.GetStudents(_studentsQueryDto);

            // Assert
            response.ShouldHaveEmptyPaginationData();
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

            _studentsQueryDto.Name = expectedStudentToFind.Name;
            _studentsQueryDto.GPAQueryDto = gpaQueryDto;

            // Act
            var response = await _studentRepository.GetStudents(_studentsQueryDto);
            response.Page.Should().Be(1);
            response.PageSize.Should().BeGreaterThan(0);
            response.TotalCount.Should().BeGreaterThan(0);

            var items = response.Items;
            // Assert
            items.Should().NotBeNullOrEmpty();
            items.Should().ContainEquivalentOf(expectedStudentToFind);
            items.Should().AllSatisfy(s => s.GPA.Should().BeNull());
            items.ShouldBeAscendingOrderedBySId();
        }

        [Test]
        public async Task GetStudents_ShouldNotMatch_OnNullGpa_WhenIsNull_False()
        {
            // Arrange
            var expectedStudentToFind = ExpectedStudentToFindMatchingName;

            _studentsQueryDto.Name = expectedStudentToFind.Name;
            _studentsQueryDto.GPAQueryDto = new GPAQueryDto() { GPA = new() { IsNull = false } };

            // Act
            var response = await _studentRepository.GetStudents(_studentsQueryDto);

            var items = response.Items;
            // Assert
            response.Should().NotBeNull();

            if (items.Count > 0)
            {
                items.Should().NotBeNull().And.HaveCountGreaterThan(0);
                items.Should().NotContainEquivalentOf(expectedStudentToFind);
                items.Should().AllSatisfy(s => s.GPA.Should().NotBeNull());
                items.ShouldBeAscendingOrderedBySId();
            }
        }

        [TestCase(false)]
        [TestCase(null)]
        public async Task GetStudents_ShouldMatch_OnGpa_WhenIsNull_False_OrNull(bool? isNull)
        {
            // Arrange
            var expectedStudentToFind = ExpectedStudentToFindMatchingNameAndGpa;

            _studentsQueryDto.Name = expectedStudentToFind.Name;
            _studentsQueryDto.GPAQueryDto = new GPAQueryDto() { GPA = new() { IsNull = isNull } };

            // Act
            var response = await _studentRepository.GetStudents(_studentsQueryDto);

            var items = response.Items;

            // Assert
            response.ShouldHaveSingleItemPaginationData();

            items.Should().ContainEquivalentOf(expectedStudentToFind);
            items.Should().AllSatisfy(s => s.GPA.Should().NotBeNull());
            items.ShouldBeAscendingOrderedBySId();
        }

        [Test]
        public async Task GetStudents_ShouldNotMatch_OnGpa_WhenIsNull_True()
        {
            // Arrange
            _studentsQueryDto.Name = ExpectedStudentToFindMatchingNameAndGpa.Name;
            _studentsQueryDto.GPAQueryDto = new GPAQueryDto() { GPA = new() { IsNull = true } };

            // Act

            var response = await _studentRepository.GetStudents(_studentsQueryDto);

            // Assert
            response.ShouldHaveEmptyPaginationData();
        }

        // Order by and sort column tests
        private static Func<StudentDto, object?> _sIdSelector = x => x.SId;
        private static Func<StudentDto, object?> _nameSelector = x => x.Name;
        private static Func<StudentDto, object?> _gpaSelector = x => x.GPA;
        private static IEnumerable<TestCaseData> GetStudents_ShouldOrderByAscendingTestCases()
        {

            yield return new TestCaseData(SortOrder.ASC, APIConstants.Student.SId, _sIdSelector);
            yield return new TestCaseData(SortOrder.ASC, APIConstants.Student.Name, _nameSelector);
            yield return new TestCaseData(SortOrder.ASC, APIConstants.Student.GPA, _gpaSelector);
        }

        [TestCaseSource(nameof(GetStudents_ShouldOrderByAscendingTestCases))]
        public async Task GetStudents_ShouldOrderByAscending(SortOrder? orderBy, string? sortColumn, Func<StudentDto, object?> expectedColumnSelector)
        {
            // Act
            var response = await _studentRepository.GetStudents(new GetStudentsQueryDto() { OrderBy = orderBy, SortColumn = sortColumn, Page = 1, PageSize = 100});

            // Assert

            response.ShouldHavePaginationData(expectedPageSize: 100);
            response.Items.ShouldBeInAscendingOrder(expectedColumnSelector);
        }

        private static IEnumerable<TestCaseData> GetStudents_ShouldOrderByDescendingTestCases()
        {
            yield return new TestCaseData(SortOrder.DESC, APIConstants.Student.SId, _sIdSelector);
            yield return new TestCaseData(SortOrder.DESC, APIConstants.Student.Name, _nameSelector);
            yield return new TestCaseData(SortOrder.DESC, APIConstants.Student.GPA, _gpaSelector);
        }

        [TestCaseSource(nameof(GetStudents_ShouldOrderByDescendingTestCases))]
        public async Task GetStudents_ShouldOrderByDescending(SortOrder? orderBy, string? sortColumn, Func<StudentDto, object?> expectedColumnSelector)
        {
            // Act
            var response = await _studentRepository.GetStudents(new GetStudentsQueryDto() { OrderBy = orderBy, SortColumn = sortColumn, Page = 1, PageSize = 100 });

            // Assert

            response.ShouldHavePaginationData(expectedPageSize: 100);
            response.Items.ShouldBeInDescendingOrder(expectedColumnSelector);
        }
        
        private static IEnumerable<TestCaseData> AscendingWhenFilteringTestCases()
        {
            yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", OrderBy = SortOrder.ASC, SortColumn = APIConstants.Student.SId }, _sIdSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", GPAQueryDto = new GPAQueryDto { GPA = new() { IsNull = false } }, OrderBy = SortOrder.ASC, SortColumn = APIConstants.Student.SId }, _sIdSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { GPAQueryDto = new GPAQueryDto { GPA = new() { IsNull = false } }, OrderBy = SortOrder.ASC, SortColumn = APIConstants.Student.SId }, _sIdSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { GPAQueryDto = new GPAQueryDto { GPA = new() { Gt = 1 } }, OrderBy = SortOrder.ASC, SortColumn = APIConstants.Student.SId }, _sIdSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", OrderBy = SortOrder.ASC, SortColumn = APIConstants.Student.SId }, _sIdSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", OrderBy = SortOrder.ASC, SortColumn = APIConstants.Student.Name }, _nameSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", OrderBy = SortOrder.ASC, SortColumn = APIConstants.Student.GPA }, _gpaSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { SId = 1, OrderBy = SortOrder.ASC, SortColumn = APIConstants.Student.SId }, _sIdSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { SId = 1, OrderBy = SortOrder.ASC, SortColumn = APIConstants.Student.Name }, _nameSelector);
        }

        [TestCaseSource(nameof(AscendingWhenFilteringTestCases))]
        public async Task GetStudents_ShouldOrderByAscendingWhenFiltering(GetStudentsQueryDto queryDto, Func<StudentDto, object?> expectedColumnSelector)
        {
            // Arrange
            queryDto.Page = 1;
            queryDto.PageSize = 10;
            // Act
            var response = await _studentRepository.GetStudents(queryDto);

            // Assert
            response.ShouldHavePaginationData();
            response.Items.ShouldBeInAscendingOrder(expectedColumnSelector);
        }

        private static IEnumerable<TestCaseData> DescendingWhenFilteringTestCases()
        {
            yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", OrderBy = SortOrder.DESC, SortColumn = APIConstants.Student.SId }, _sIdSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", GPAQueryDto = new GPAQueryDto { GPA = new() { IsNull = false } }, OrderBy = SortOrder.DESC, SortColumn = APIConstants.Student.SId }, _sIdSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { GPAQueryDto = new GPAQueryDto { GPA = new() { IsNull = false } }, OrderBy = SortOrder.DESC, SortColumn = APIConstants.Student.SId }, _sIdSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { GPAQueryDto = new GPAQueryDto { GPA = new() { Gt = 1 } }, OrderBy = SortOrder.DESC, SortColumn = APIConstants.Student.SId }, _sIdSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", OrderBy = SortOrder.DESC, SortColumn = APIConstants.Student.SId }, _sIdSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", OrderBy = SortOrder.DESC, SortColumn = APIConstants.Student.Name }, _nameSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", OrderBy = SortOrder.DESC, SortColumn = APIConstants.Student.GPA }, _gpaSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { SId = 1, OrderBy = SortOrder.DESC, SortColumn = APIConstants.Student.SId }, _sIdSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { SId = 1, OrderBy = SortOrder.DESC, SortColumn = APIConstants.Student.Name }, _nameSelector);
        }

        [TestCaseSource(nameof(DescendingWhenFilteringTestCases))]
        public async Task GetStudents_ShouldOrderByDescending_WhenFiltering(GetStudentsQueryDto queryDto, Func<StudentDto, object?> expectedColumnSelector)
        {
            // Arrange
            queryDto.Page = 1;
            queryDto.PageSize = 10;

            // Act
            var response = await _studentRepository.GetStudents(queryDto);

            // Assert
            response.ShouldHavePaginationData();
            response.Items.ShouldBeInDescendingOrder(expectedColumnSelector);
        }
    }
}
