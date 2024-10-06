using FluentAssertions;
using Harri.SchoolDemoAPI.Models;
using Harri.SchoolDemoAPI.Models.Dto;
using Harri.SchoolDemoAPI.Models.Enums;
using Harri.SchoolDemoAPI.Tests.Integration.TestBase;

namespace Harri.SchoolDemoAPI.Tests.Integration
{
    public class StudentRepositoryQueryPagedTests : StudentRepositoryTestBase
        //TODO SID TESTS
    {
        // Order by and sort column paged tests
        private static Func<StudentDto, object?> _sIdSelector = x => x.SId;
        private static Func<StudentDto, object?> _nameSelector = x => x.Name;
        private static Func<StudentDto, object?> _gpaSelector = x => x.GPA;

        private static IEnumerable<TestCaseData> GetStudents_AllPagesAscendingTestCases()
        {
            yield return new TestCaseData(SortOrder.ASC, APIConstants.Student.SId, _sIdSelector);
            yield return new TestCaseData(SortOrder.ASC, APIConstants.Student.Name, _nameSelector);
            yield return new TestCaseData(SortOrder.ASC, APIConstants.Student.GPA, _gpaSelector);
        }

        private static IEnumerable<TestCaseData> GetStudents_AllPagesDescendingTestCases()
        {
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
                allPageItems.ShouldBeInAscendingOrder(expectedColumnSelector);
            }
            else if (orderBy is SortOrder.DESC)
            {
                allPageItems.ShouldBeInDescendingOrder(expectedColumnSelector);
            }
        }

        // Filtering paged tests
        private static IEnumerable<TestCaseData> GetStudents_AllPagesAscendingWhenFilteringTestCases()
        {
            yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", GPAQueryDto = new GPAQueryDto { GPA = new() { IsNull = false } }, OrderBy = SortOrder.ASC, SortColumn = APIConstants.Student.SId }, _sIdSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { GPAQueryDto = new GPAQueryDto { GPA = new() { IsNull = false } }, OrderBy = SortOrder.ASC, SortColumn = APIConstants.Student.SId }, _sIdSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { GPAQueryDto = new GPAQueryDto { GPA = new() { Gt = 1 } }, OrderBy = SortOrder.ASC, SortColumn = APIConstants.Student.SId }, _sIdSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", OrderBy = SortOrder.ASC, SortColumn = APIConstants.Student.SId }, _sIdSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", OrderBy = SortOrder.ASC, SortColumn = APIConstants.Student.Name }, _nameSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", OrderBy = SortOrder.ASC, SortColumn = APIConstants.Student.GPA }, _gpaSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { SId = 1, OrderBy = SortOrder.ASC, SortColumn = APIConstants.Student.SId }, _sIdSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { SId = 1, OrderBy = SortOrder.ASC, SortColumn = APIConstants.Student.Name }, _nameSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { SId = 1, OrderBy = SortOrder.ASC, SortColumn = APIConstants.Student.GPA }, _gpaSelector);
        }

        private static IEnumerable<TestCaseData> GetStudents_AllPagesDescendingWhenFilteringTestCases()
        {
            yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", GPAQueryDto = new GPAQueryDto { GPA = new() { IsNull = false } }, OrderBy = SortOrder.DESC, SortColumn = APIConstants.Student.SId }, _sIdSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { GPAQueryDto = new GPAQueryDto { GPA = new() { IsNull = false } }, OrderBy = SortOrder.DESC, SortColumn = APIConstants.Student.SId }, _sIdSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { GPAQueryDto = new GPAQueryDto { GPA = new() { Gt = 1 } }, OrderBy = SortOrder.DESC, SortColumn = APIConstants.Student.SId }, _sIdSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", OrderBy = SortOrder.DESC, SortColumn = APIConstants.Student.SId }, _sIdSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", OrderBy = SortOrder.DESC, SortColumn = APIConstants.Student.Name }, _nameSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", OrderBy = SortOrder.DESC, SortColumn = APIConstants.Student.GPA }, _gpaSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { SId = 1, OrderBy = SortOrder.DESC, SortColumn = APIConstants.Student.SId }, _sIdSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { SId = 1, OrderBy = SortOrder.DESC, SortColumn = APIConstants.Student.Name }, _nameSelector);
            yield return new TestCaseData(new GetStudentsQueryDto() { SId = 1, OrderBy = SortOrder.DESC, SortColumn = APIConstants.Student.GPA }, _gpaSelector);
        }

        [TestCaseSource(nameof(GetStudents_AllPagesAscendingWhenFilteringTestCases))]
        [TestCaseSource(nameof(GetStudents_AllPagesDescendingWhenFilteringTestCases))]
        [NonParallelizable]
        public async Task GetStudents_AllPagesShouldBeOrdered_WhenFiltering(GetStudentsQueryDto queryDto, Func<StudentDto, object?> expectedColumnSelector)
        {
            // Arrange
            // Act
            List<StudentDto> allPageItems = await GetAllPages(queryDto.OrderBy, queryDto.SortColumn, queryDto.SId, queryDto.Name, queryDto.GPAQueryDto);

            // Assert
            if (queryDto.OrderBy is SortOrder.ASC)
            {
                allPageItems.ShouldBeInAscendingOrder(expectedColumnSelector);
            }
            else if (queryDto.OrderBy is SortOrder.DESC)
            {
                allPageItems.ShouldBeInDescendingOrder(expectedColumnSelector);
            }
        }


        // These tests that make multiple page requests and combine together to assert the results of all pages. They are non parellelizable as
        // changes made by other tests while running will cause flakiness in the results. Tests like this should not be run against a shared database for the same reason
        public static async Task<List<StudentDto>> GetAllPages(SortOrder? orderBy, string? sortColumn, int? sId = null, string? name = null, GPAQueryDto? gpaQueryDto = null)
        {
            var allPageItems = new List<StudentDto>();
            var page = 1;
            var pageSize = 100;
            int? totalCount = null;
            PagedList<StudentDto> response;
            do
            {
                response = await _studentRepository.GetStudents(new GetStudentsQueryDto() { SId = sId, Name = name, GPAQueryDto = gpaQueryDto, OrderBy = orderBy, SortColumn = sortColumn, Page = page, PageSize = pageSize });

                if (totalCount is null)
                {
                    totalCount = response.TotalCount;
                }

                if (response.HasNextPage)
                {
                    response.ShouldHavePaginationData(expectedPage: page, expectedPageSize: pageSize);
                }
                allPageItems.AddRange(response.Items);

                page++;
            } while (response.HasNextPage); // Get all pages

            allPageItems.Count().Should().Be(totalCount);
            return allPageItems;
        }
    }
}
