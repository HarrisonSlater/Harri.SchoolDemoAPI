using FluentAssertions;
using Harri.SchoolDemoAPI.Models.Dto;
using Harri.SchoolDemoAPI.Tests.Common;
using PactNet.Matchers;

namespace Harri.SchoolDemoAPI.Tests.Contract.Consumer.Helpers
{
    internal static class StudentsQueryApiTestHelper
    {

        public static void AssertStudentsResponseIsCorrect(PagedList<StudentDto> response)
        {
            response.Should().NotBeNull();

            var students = response.Items;
            // Client Assertions
            students.Should().NotBeNull().And.HaveCountGreaterThan(0);

            foreach (var student in students)
            {
                student.SId.Should().NotBeNull().And.BeGreaterThan(0);
                student.Name.Should().NotBeNullOrWhiteSpace();
                student.GPA.Should().NotBeNull().And.BeGreaterThan(3.7m);
            }

            response.Page.Should().Be(1);
            response.PageSize.Should().Be(3);
            response.TotalCount.Should().Be(3);
        }

    }
}