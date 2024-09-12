using Azure;
using FluentAssertions;
using Harri.SchoolDemoAPI.Models;
using Harri.SchoolDemoAPI.Models.Dto;
using Harri.SchoolDemoAPI.Models.Enums;
using Harri.SchoolDemoAPI.Tests.Integration.TestBase;
using NUnit.Framework.Internal;
using System.Globalization;

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

        private GetStudentsQueryDto _studentsQueryDto;

        [SetUp]
        public void SetUp()
        {
            _studentsQueryDto = new GetStudentsQueryDto() { Page = 1, PageSize = 10 };
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
        private static IEnumerable<TestCaseData> GetStudents_AllPagesShouldOrderByAscendingTestCases()
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

        [TestCaseSource(nameof(GetStudents_AllPagesShouldOrderByAscendingTestCases))]
        [NonParallelizable] 
        // These tests that make multiple page requests and combine together to assert the results of all pages are non parellelizable as
        // changes made by other tests while running will cause flakiness in the results. These tests should not be run against a shared database for the same reason
        public async Task GetStudents_ShouldOrderByAscending(SortOrder? orderBy, string? sortColumn, Func<StudentDto, object?> expectedColumnSelector)
        {
            // Act
            var allPageItems = new List<StudentDto>();
            var page = 1;
            var pageSize = 100;
            int? totalCount = null;
            PagedList<StudentDto> response;
            do
            {
                response = await _studentRepository.GetStudents(new GetStudentsQueryDto() { OrderBy = orderBy, SortColumn = sortColumn, Page = page, PageSize = pageSize });

                if (totalCount is null)
                {
                    totalCount = response.TotalCount;
                }

                if (response.HasNextPage)
                {
                    response.Items.Count().Should().Be(pageSize);
                    response.Page.Should().Be(page);
                    response.PageSize.Should().Be(pageSize);
                }
                allPageItems.AddRange(response.Items);

                page++;
            } while (response.HasNextPage); // Get all pages

            // Assert
            Assertions.AssertInAscendingOrder(response.Items, expectedColumnSelector);
        }

        private static IEnumerable<TestCaseData> GetStudents_ShouldOrderByDescendingTestCases()
        {
            //TODO
            yield return new TestCaseData(SortOrder.DESC, null, _sIdSelector);
            yield return new TestCaseData(SortOrder.DESC, APIConstants.Student.SId, _sIdSelector);
            yield return new TestCaseData(SortOrder.DESC, APIConstants.Student.Name, _nameSelector);
            yield return new TestCaseData(SortOrder.DESC, APIConstants.Student.GPA, _gpaSelector);
        }

        [TestCaseSource(nameof(GetStudents_ShouldOrderByDescendingTestCases))]
        [NonParallelizable]
        public async Task GetStudents_ShouldOrderByDescending(SortOrder? orderBy, string? sortColumn, Func<StudentDto, object?> expectedColumnSelector)
        {
            // Act
            var response = await _studentRepository.GetStudents(new GetStudentsQueryDto() { OrderBy = orderBy, SortColumn = sortColumn, Page = 1, PageSize = 100 });

            // Assert
            Assertions.AssertPageResponse(response, expectedPageSize: 100);
            Assertions.AssertInDescendingOrder(response.Items, expectedColumnSelector);
        }
        
        //Filtering paged tests
        //private static IEnumerable<TestCaseData> AscendingWhenFilteringTestCases()
        //{
        //    yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", OrderBy = SortOrder.ASC }, _sIdSelector);
        //    yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", GPAQueryDto = new GPAQueryDto { GPA = new() { IsNull = false } }, OrderBy = SortOrder.ASC }, _sIdSelector);
        //    yield return new TestCaseData(new GetStudentsQueryDto() { GPAQueryDto = new GPAQueryDto { GPA = new() { IsNull = false } }, OrderBy = SortOrder.ASC }, _sIdSelector);
        //    yield return new TestCaseData(new GetStudentsQueryDto() { GPAQueryDto = new GPAQueryDto { GPA = new() { Gt = 1 } }, OrderBy = SortOrder.ASC }, _sIdSelector);
        //    yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", OrderBy = SortOrder.ASC, SortColumn = APIConstants.Student.SId }, _sIdSelector);
        //    yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", OrderBy = SortOrder.ASC, SortColumn = APIConstants.Student.Name }, _nameSelector);
        //    yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", OrderBy = SortOrder.ASC, SortColumn = APIConstants.Student.GPA }, _gpaSelector);
        //}

        //[TestCaseSource(nameof(AscendingWhenFilteringTestCases))]
        //public async Task GetStudents_ShouldOrderByAscendingWhenFiltering(GetStudentsQueryDto queryDto, Func<StudentDto, object?> expectedColumnSelector)
        //{
        //    // Arrange
        //    queryDto.Page = 1;
        //    queryDto.PageSize = 10;
        //    // Act
        //    var response = await _studentRepository.GetStudents(queryDto);

        //    // Assert
        //    AssertDefaultPageResponse(response);
        //    AssertInAscendingOrder(response.Items, expectedColumnSelector);
        //}

        //private static IEnumerable<TestCaseData> DescendingWhenFilteringTestCases()
        //{
        //    yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", OrderBy = SortOrder.DESC }, _sIdSelector);
        //    yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", GPAQueryDto = new GPAQueryDto { GPA = new() { IsNull = false } }, OrderBy = SortOrder.DESC }, _sIdSelector);
        //    yield return new TestCaseData(new GetStudentsQueryDto() { GPAQueryDto = new GPAQueryDto { GPA = new() { IsNull = false } }, OrderBy = SortOrder.DESC }, _sIdSelector);
        //    yield return new TestCaseData(new GetStudentsQueryDto() { GPAQueryDto = new GPAQueryDto { GPA = new() { Gt = 1 } }, OrderBy = SortOrder.DESC }, _sIdSelector);
        //    yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", OrderBy = SortOrder.DESC, SortColumn = APIConstants.Student.SId }, _sIdSelector);
        //    yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", OrderBy = SortOrder.DESC, SortColumn = APIConstants.Student.Name }, _nameSelector);
        //    yield return new TestCaseData(new GetStudentsQueryDto() { Name = "Smith", OrderBy = SortOrder.DESC, SortColumn = APIConstants.Student.GPA }, _gpaSelector);
        //}

        //[TestCaseSource(nameof(DescendingWhenFilteringTestCases))]
        //public async Task GetStudents_ShouldOrderByDescending_WhenFiltering(GetStudentsQueryDto queryDto, Func<StudentDto, object?> expectedColumnSelector)
        //{
        //    // Arrange
        //    queryDto.Page = 1;
        //    queryDto.PageSize = 10;

        //    // Act
        //    var response = await _studentRepository.GetStudents(queryDto);

        //    // Assert
        //    AssertDefaultPageResponse(response);
        //    AssertInDescendingOrder(response.Items, expectedColumnSelector);
        //}
    }
}
