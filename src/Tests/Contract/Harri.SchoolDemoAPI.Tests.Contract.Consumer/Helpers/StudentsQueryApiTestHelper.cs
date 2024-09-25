using FluentAssertions;
using Harri.SchoolDemoAPI.Models.Dto;
using PactNet.Matchers;

namespace Harri.SchoolDemoAPI.Tests.Contract.Consumer.Helpers
{
    internal static class StudentsQueryApiTestHelper
    {
        public static dynamic ExpectedPagedStudentsJsonBody = 
            new
            {
                items = new List<object>()
                {
                    new
                    {
                        sId = Match.Equality(1),
                        name = Match.Equality("Test student 1"),
                        GPA = Match.Equality(3.99)
                    },
                    new
                    {
                        sId = Match.Equality(2),
                        name = Match.Equality("Test student 2"),
                        GPA = Match.Equality(3.89)
                    },
                    new
                    {
                        sId = Match.Equality(3),
                        name = Match.Equality("Test student 3"),
                        GPA = Match.Equality(3.79)
                    },
                },
                page = 1,
                pageSize = 3,
                totalCount = 3
            };

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