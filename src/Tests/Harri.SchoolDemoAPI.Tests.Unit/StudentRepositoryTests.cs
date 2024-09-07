using FluentAssertions;
using Harri.SchoolDemoAPI.Models;
using Harri.SchoolDemoAPI.Repository;

namespace Harri.SchoolDemoAPI.Tests.Unit
{
    [TestFixture]
    public class StudentRepositoryTests
    {
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
    }
}
