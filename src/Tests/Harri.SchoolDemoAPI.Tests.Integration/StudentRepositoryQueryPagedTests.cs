using FluentAssertions;
using Harri.SchoolDemoAPI.Models;
using Harri.SchoolDemoAPI.Models.Dto;
using Harri.SchoolDemoAPI.Models.Enums;
using Harri.SchoolDemoAPI.Tests.Integration.TestBase;

namespace Harri.SchoolDemoAPI.Tests.Integration
{
    public class StudentRepositoryQueryPagedTests : StudentRepositoryTestBase
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


        [SetUp]
        public void SetUp()
        {
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
        public async Task TearDown()
        {
            await CleanUpTestStudent(_studentToMatchNameId);
            await CleanUpTestStudent(_studentToMatchGpaId);
            await CleanUpTestStudent(_studentToMatchNameAndGpaId);
            await CleanUpTestStudent(_studentToMatchNameSpecialId);
            await CleanUpTestStudent(_studentToMatchNameUnicodeId);
        }

        // Order by and sort column paged tests
        private static Func<StudentDto, object?> _sIdSelector = x => x.SId;
        private static Func<StudentDto, object?> _nameSelector = x => x.Name;
        private static Func<StudentDto, object?> _gpaSelector = x => x.GPA;
        private static IEnumerable<TestCaseData> GetStudents_AllPagesAscendingTestCases()
        {
            yield return new TestCaseData(null, null, _sIdSelector);
            yield return new TestCaseData(null, APIConstants.Student.SId, _sIdSelector);
            yield return new TestCaseData(null, APIConstants.Student.Name, _nameSelector);
            yield return new TestCaseData(null, APIConstants.Student.GPA, _gpaSelector);
            yield return new TestCaseData(SortOrder.ASC, null, _sIdSelector);
            yield return new TestCaseData(SortOrder.ASC, APIConstants.Student.SId, _sIdSelector);
            yield return new TestCaseData(SortOrder.ASC, APIConstants.Student.Name, _nameSelector);
            yield return new TestCaseData(SortOrder.ASC, APIConstants.Student.GPA, _gpaSelector);
        }

        private static IEnumerable<TestCaseData> GetStudents_AllPagesDescendingTestCases()
        {
            yield return new TestCaseData(SortOrder.DESC, null, _sIdSelector);
            yield return new TestCaseData(SortOrder.DESC, APIConstants.Student.SId, _sIdSelector);
            yield return new TestCaseData(SortOrder.DESC, APIConstants.Student.Name, _nameSelector);
            yield return new TestCaseData(SortOrder.DESC, APIConstants.Student.GPA, _gpaSelector);
        }

        [TestCaseSource(nameof(GetStudents_AllPagesAscendingTestCases))]
        [TestCaseSource(nameof(GetStudents_AllPagesDescendingTestCases))]
        [NonParallelizable] 
        public async Task GetStudents_AllPagesShouldBeOrdered(SortOrder? orderBy, string? sortColumn, Func<StudentDto, object?> expectedColumnSelector)
        {
            // Act
            List<StudentDto> allPageItems = await GetAllPages(orderBy, sortColumn);

            // Assert
            if (orderBy is SortOrder.ASC)
            {
                Assertions.AssertInAscendingOrder(allPageItems, expectedColumnSelector);
            }
            else if (orderBy is SortOrder.DESC)
            {
                Assertions.AssertInDescendingOrder(allPageItems, expectedColumnSelector);
            }
        }

        // Filtering paged tests
        private static IEnumerable<TestCaseData> GetStudents_AllPagesAscendingWhenFilteringTestCases()
        {
            yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", OrderBy = SortOrder.ASC }, _sIdSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", GPAQueryDto = new GPAQueryDto { GPA = new() { IsNull = false } }, OrderBy = SortOrder.ASC }, _sIdSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { GPAQueryDto = new GPAQueryDto { GPA = new() { IsNull = false } }, OrderBy = SortOrder.ASC }, _sIdSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { GPAQueryDto = new GPAQueryDto { GPA = new() { Gt = 1 } }, OrderBy = SortOrder.ASC }, _sIdSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", OrderBy = SortOrder.ASC, SortColumn = APIConstants.Student.SId }, _sIdSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", OrderBy = SortOrder.ASC, SortColumn = APIConstants.Student.Name }, _nameSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", OrderBy = SortOrder.ASC, SortColumn = APIConstants.Student.GPA }, _gpaSelector);
        }

        private static IEnumerable<TestCaseData> GetStudents_AllPagesDescendingWhenFilteringTestCases()
        {
            yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", OrderBy = SortOrder.DESC }, _sIdSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", GPAQueryDto = new GPAQueryDto { GPA = new() { IsNull = false } }, OrderBy = SortOrder.DESC }, _sIdSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { GPAQueryDto = new GPAQueryDto { GPA = new() { IsNull = false } }, OrderBy = SortOrder.DESC }, _sIdSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { GPAQueryDto = new GPAQueryDto { GPA = new() { Gt = 1 } }, OrderBy = SortOrder.DESC }, _sIdSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", OrderBy = SortOrder.DESC, SortColumn = APIConstants.Student.SId }, _sIdSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", OrderBy = SortOrder.DESC, SortColumn = APIConstants.Student.Name }, _nameSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", OrderBy = SortOrder.DESC, SortColumn = APIConstants.Student.GPA }, _gpaSelector);
        }

        [TestCaseSource(nameof(GetStudents_AllPagesAscendingWhenFilteringTestCases))]
        [TestCaseSource(nameof(GetStudents_AllPagesDescendingWhenFilteringTestCases))]
        [NonParallelizable]
        public async Task GetStudents_AllPagesShouldBeOrdered_WhenFiltering(GetStudentsQueryDto queryDto, Func<StudentDto, object?> expectedColumnSelector)
        {
            // Arrange
            // Act
            List<StudentDto> allPageItems = await GetAllPages(queryDto.OrderBy, queryDto.SortColumn, queryDto.Name, queryDto.GPAQueryDto);

            // Assert
            if (queryDto.OrderBy is SortOrder.ASC)
            {
                Assertions.AssertInAscendingOrder(allPageItems, expectedColumnSelector);
            }
            else if (queryDto.OrderBy is SortOrder.DESC)
            {
                Assertions.AssertInDescendingOrder(allPageItems, expectedColumnSelector);
            }
        }

        //TODO strict test with strict assertions

        // These tests that make multiple page requests and combine together to assert the results of all pages. They are non parellelizable as
        // changes made by other tests while running will cause flakiness in the results. Tests like this should not be run against a shared database for the same reason
        private static async Task<List<StudentDto>> GetAllPages(SortOrder? orderBy, string? sortColumn, string? name = null, GPAQueryDto? gpaQueryDto = null)
        {
            var allPageItems = new List<StudentDto>();
            var page = 1;
            var pageSize = 100;
            int? totalCount = null;
            PagedList<StudentDto> response;
            do
            {
                response = await _studentRepository.GetStudents(new GetStudentsQueryDto() { Name = name, GPAQueryDto = gpaQueryDto, OrderBy = orderBy, SortColumn = sortColumn, Page = page, PageSize = pageSize });

                if (totalCount is null)
                {
                    totalCount = response.TotalCount;
                }

                if (response.HasNextPage)
                {
                    Assertions.AssertPageResponse(response, expectedPage: page, expectedPageSize: pageSize);
                }
                allPageItems.AddRange(response.Items);

                page++;
            } while (response.HasNextPage); // Get all pages

            allPageItems.Count().Should().Be(totalCount);
            return allPageItems;
        }

    }
}
