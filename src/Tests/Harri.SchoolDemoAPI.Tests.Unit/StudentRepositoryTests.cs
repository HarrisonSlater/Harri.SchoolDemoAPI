using FluentAssertions;
using Harri.SchoolDemoAPI.Models;
using Harri.SchoolDemoAPI.Models.Dto;
using Harri.SchoolDemoAPI.Models.Enums;
using Harri.SchoolDemoAPI.Repository;
using Moq;

namespace Harri.SchoolDemoAPI.Tests.Unit
{
    [TestFixture]
    public class StudentRepositoryTests
    {
        private StudentRepository _studentRepository;

        [SetUp]
        public void SetUp()
        {
            _studentRepository = new StudentRepository(Mock.Of<IDbConnectionFactory>());
        }

        [TestCase("sid", APIConstants.Student.SId)]
        [TestCase("Sid", APIConstants.Student.SId)]
        [TestCase("SId", APIConstants.Student.SId)]
        [TestCase("SID", APIConstants.Student.SId)]
        [TestCase("name", APIConstants.Student.Name)]
        [TestCase("Name", APIConstants.Student.Name)]
        [TestCase("NAME", APIConstants.Student.Name)]
        [TestCase("gpa", APIConstants.Student.GPA)]
        [TestCase("Gpa", APIConstants.Student.GPA)]
        [TestCase("GPA", APIConstants.Student.GPA)]
        public void GetCleanSortColumn_ShouldWorkForValidColumns(string? sortColumnToClean, string expectedString)
        {
            StudentRepository.GetCleanSortColumn(sortColumnToClean).Should().Be(expectedString);
        }

        [TestCase("sids")]
        [TestCase("names")]
        [TestCase("gp")]
        [TestCase("asdf")]
        public void GetCleanSortColumn_ShouldThrowForInvalidColumns(string? sortColumnToClean)
        {
            Action action = () => StudentRepository.GetCleanSortColumn(sortColumnToClean);
            action.Should().Throw<ArgumentException>();
        }

        [TestCase(null, 10, SortOrder.ASC, APIConstants.Student.SId)]
        [TestCase(1, null, SortOrder.ASC, APIConstants.Student.SId)]
        [TestCase(1, 10, null, APIConstants.Student.SId)]
        [TestCase(1, 10, SortOrder.ASC, null)]
        [TestCase(null, null, SortOrder.ASC, APIConstants.Student.SId)]
        public async Task GetStudents_ShouldGuardAgainstNullPaginationArguments(int? page, int? pageSize, SortOrder? orderBy, string? sortColumn)
        {
            // Arrange
            var queryDto = new GetStudentsQueryDto()
            {
                Page = page,
                PageSize = pageSize,
                OrderBy = orderBy,
                SortColumn = sortColumn
            };

            // Act
            var action = () => _studentRepository.GetStudents(queryDto);

            // Assert
            await action.Should().ThrowExactlyAsync<ArgumentNullException>();
        }
    }
}
