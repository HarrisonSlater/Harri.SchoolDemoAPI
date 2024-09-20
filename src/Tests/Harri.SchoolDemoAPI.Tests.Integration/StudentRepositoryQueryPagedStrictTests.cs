using FluentAssertions;
using Harri.SchoolDemoAPI.Models;
using Harri.SchoolDemoAPI.Models.Dto;
using Harri.SchoolDemoAPI.Models.Enums;
using Harri.SchoolDemoAPI.Tests.Integration.TestBase;

namespace Harri.SchoolDemoAPI.Tests.Integration
{
    public class StudentRepositoryQueryPagedStrictTests : StudentRepositoryTestBase
    {

        private static readonly int TestPageSize = 4;
        private static readonly int TotalTestStudents = 16;
        private static Guid TestStudentNamePrefix;

        private static List<StudentDto> CreatedStudents = new();


        [SetUp]
        public void SetUp()
        {
        }

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            TestStudentNamePrefix = Guid.NewGuid();

            for (int i = 0; i < TotalTestStudents; i++)
            {
                var newStudent = new NewStudentDto() { Name = $"{TestStudentNamePrefix} Test Student {i + 1}", GPA = i + 1 };
                var newStudentId = await _studentRepository.AddStudent(newStudent);

                CreatedStudents.Add(GetStudentDtoFor(newStudentId, newStudent));
            }
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            for (int i = 0; i < TotalTestStudents; i++)
            {
                await CleanUpTestStudent(CreatedStudents[i].SId!.Value);
            }
        }

        [Test]
        public async Task GetStudents_ShouldWorkAsExpectedWithAFixedNumberOfStudents()
        {
            // Arrange
            // Act
            var studentsResponse1 = await _studentRepository.GetStudents(new GetStudentsQueryDto() { Name = TestStudentNamePrefix.ToString(), OrderBy = SortOrder.ASC, SortColumn = APIConstants.Student.SId, Page = 1, PageSize = 4 });
            var studentsResponse2 = await _studentRepository.GetStudents(new GetStudentsQueryDto() { Name = TestStudentNamePrefix.ToString(), OrderBy = SortOrder.ASC, SortColumn = APIConstants.Student.SId, Page = 2, PageSize = TestPageSize });
            var studentsResponse3 = await _studentRepository.GetStudents(new GetStudentsQueryDto() { Name = TestStudentNamePrefix.ToString(), OrderBy = SortOrder.ASC, SortColumn = APIConstants.Student.SId, Page = 3, PageSize = TestPageSize  });
            var studentsResponse4 = await _studentRepository.GetStudents(new GetStudentsQueryDto() { Name = TestStudentNamePrefix.ToString(), OrderBy = SortOrder.ASC, SortColumn = APIConstants.Student.SId, Page = 4, PageSize = TestPageSize });

            // Assert
            studentsResponse1.Page.Should().Be(1);
            studentsResponse1.HasPreviousPage.Should().BeFalse();
            studentsResponse1.HasNextPage.Should().BeTrue();
            AssertPageInfo(studentsResponse1);

            studentsResponse2.Page.Should().Be(2);
            studentsResponse2.HasPreviousPage.Should().BeTrue();
            studentsResponse2.HasNextPage.Should().BeTrue();
            AssertPageInfo(studentsResponse2);

            studentsResponse3.Page.Should().Be(3);
            studentsResponse3.HasPreviousPage.Should().BeTrue();
            studentsResponse3.HasNextPage.Should().BeTrue();
            AssertPageInfo(studentsResponse3);

            studentsResponse4.Page.Should().Be(4);
            studentsResponse4.HasPreviousPage.Should().BeTrue();
            studentsResponse4.HasNextPage.Should().BeFalse();
            AssertPageInfo(studentsResponse4);

            var allStudents = new List<StudentDto>();
            allStudents.AddRange(studentsResponse1.Items);
            allStudents.AddRange(studentsResponse2.Items);
            allStudents.AddRange(studentsResponse3.Items);
            allStudents.AddRange(studentsResponse4.Items);

            allStudents.Should().BeInAscendingOrder(s => s.SId);
            allStudents.Should().AllSatisfy(s => s.Name!.Contains(TestStudentNamePrefix.ToString()));
            allStudents.Should().AllSatisfy(s => s.GPA.Should().NotBeNull());
        }

        private void AssertPageInfo(PagedList<StudentDto> students)
        {
            students.PageSize.Should().Be(TestPageSize);
            students.Items.Should().HaveCount(TestPageSize);
            students.TotalCount.Should().Be(TotalTestStudents);
        }

        //// Order by and sort column paged tests
        //private static Func<StudentDto, object?> _sIdSelector = x => x.SId;
        //private static Func<StudentDto, object?> _nameSelector = x => x.Name;
        //private static Func<StudentDto, object?> _gpaSelector = x => x.GPA;
        //private static IEnumerable<TestCaseData> GetStudents_AllPagesAscendingTestCases()
        //{
        //    yield return new TestCaseData(null, null, _sIdSelector);
        //    yield return new TestCaseData(null, APIConstants.Student.SId, _sIdSelector);
        //    yield return new TestCaseData(null, APIConstants.Student.Name, _nameSelector);
        //    yield return new TestCaseData(null, APIConstants.Student.GPA, _gpaSelector);
        //    yield return new TestCaseData(SortOrder.ASC, null, _sIdSelector);
        //    yield return new TestCaseData(SortOrder.ASC, APIConstants.Student.SId, _sIdSelector);
        //    yield return new TestCaseData(SortOrder.ASC, APIConstants.Student.Name, _nameSelector);
        //    yield return new TestCaseData(SortOrder.ASC, APIConstants.Student.GPA, _gpaSelector);
        //}

        //private static IEnumerable<TestCaseData> GetStudents_AllPagesDescendingTestCases()
        //{
        //    yield return new TestCaseData(SortOrder.DESC, null, _sIdSelector);
        //    yield return new TestCaseData(SortOrder.DESC, APIConstants.Student.SId, _sIdSelector);
        //    yield return new TestCaseData(SortOrder.DESC, APIConstants.Student.Name, _nameSelector);
        //    yield return new TestCaseData(SortOrder.DESC, APIConstants.Student.GPA, _gpaSelector);
        //}

        //[TestCaseSource(nameof(GetStudents_AllPagesAscendingTestCases))]
        //[TestCaseSource(nameof(GetStudents_AllPagesDescendingTestCases))]
        //[NonParallelizable] 
        //public async Task GetStudents_AllPagesShouldBeOrdered(SortOrder? orderBy, string? sortColumn, Func<StudentDto, object?> expectedColumnSelector)
        //{
        //    // Act
        //    List<StudentDto> allPageItems = await GetAllPages(orderBy, sortColumn);

        //    // Assert
        //    if (orderBy is SortOrder.ASC)
        //    {
        //        Assertions.AssertInAscendingOrder(allPageItems, expectedColumnSelector);
        //    }
        //    else if (orderBy is SortOrder.DESC)
        //    {
        //        Assertions.AssertInDescendingOrder(allPageItems, expectedColumnSelector);
        //    }
        //}

        //// Filtering paged tests
        //private static IEnumerable<TestCaseData> GetStudents_AllPagesAscendingWhenFilteringTestCases()
        //{
        //    yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", OrderBy = SortOrder.ASC }, _sIdSelector);
        //    yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", GPAQueryDto = new GPAQueryDto { GPA = new() { IsNull = false } }, OrderBy = SortOrder.ASC }, _sIdSelector);
        //    yield return new TestCaseData(new GetStudentsQueryDto() { GPAQueryDto = new GPAQueryDto { GPA = new() { IsNull = false } }, OrderBy = SortOrder.ASC }, _sIdSelector);
        //    yield return new TestCaseData(new GetStudentsQueryDto() { GPAQueryDto = new GPAQueryDto { GPA = new() { Gt = 1 } }, OrderBy = SortOrder.ASC }, _sIdSelector);
        //    yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", OrderBy = SortOrder.ASC, SortColumn = APIConstants.Student.SId }, _sIdSelector);
        //    yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", OrderBy = SortOrder.ASC, SortColumn = APIConstants.Student.Name }, _nameSelector);
        //    yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", OrderBy = SortOrder.ASC, SortColumn = APIConstants.Student.GPA }, _gpaSelector);
        //}

        //private static IEnumerable<TestCaseData> GetStudents_AllPagesDescendingWhenFilteringTestCases()
        //{
        //    yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", OrderBy = SortOrder.DESC }, _sIdSelector);
        //    yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", GPAQueryDto = new GPAQueryDto { GPA = new() { IsNull = false } }, OrderBy = SortOrder.DESC }, _sIdSelector);
        //    yield return new TestCaseData(new GetStudentsQueryDto() { GPAQueryDto = new GPAQueryDto { GPA = new() { IsNull = false } }, OrderBy = SortOrder.DESC }, _sIdSelector);
        //    yield return new TestCaseData(new GetStudentsQueryDto() { GPAQueryDto = new GPAQueryDto { GPA = new() { Gt = 1 } }, OrderBy = SortOrder.DESC }, _sIdSelector);
        //    yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", OrderBy = SortOrder.DESC, SortColumn = APIConstants.Student.SId }, _sIdSelector);
        //    yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", OrderBy = SortOrder.DESC, SortColumn = APIConstants.Student.Name }, _nameSelector);
        //    yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", OrderBy = SortOrder.DESC, SortColumn = APIConstants.Student.GPA }, _gpaSelector);
        //}

        //[TestCaseSource(nameof(GetStudents_AllPagesAscendingWhenFilteringTestCases))]
        //[TestCaseSource(nameof(GetStudents_AllPagesDescendingWhenFilteringTestCases))]
        //[NonParallelizable]
        //public async Task GetStudents_AllPagesShouldBeOrdered_WhenFiltering(GetStudentsQueryDto queryDto, Func<StudentDto, object?> expectedColumnSelector)
        //{
        //    // Arrange
        //    // Act
        //    List<StudentDto> allPageItems = await GetAllPages(queryDto.OrderBy, queryDto.SortColumn, queryDto.Name, queryDto.GPAQueryDto);

        //    // Assert
        //    if (queryDto.OrderBy is SortOrder.ASC)
        //    {
        //        Assertions.AssertInAscendingOrder(allPageItems, expectedColumnSelector);
        //    }
        //    else if (queryDto.OrderBy is SortOrder.DESC)
        //    {
        //        Assertions.AssertInDescendingOrder(allPageItems, expectedColumnSelector);
        //    }
        //}
    }
}
