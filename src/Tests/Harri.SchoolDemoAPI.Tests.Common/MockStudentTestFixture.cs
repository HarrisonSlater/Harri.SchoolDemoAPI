using Harri.SchoolDemoAPI.Models.Dto;
using PactNet.Matchers;

namespace Harri.SchoolDemoAPI.Tests.Common
{
    public static class MockStudentTestFixture
    {
        // For provider
        public static List<StudentDto> MockStudentsToReturn =
        [
            new StudentDto()
            {
                SId = 1,
                Name = "Test student 1",
                GPA = 3.99m
            },
            new StudentDto()
            {
                SId = 2,
                Name = "Test student 2",
                GPA = 3.89m
            },
            new StudentDto()
            {
                SId = 3,
                Name = "Test student 3",
                GPA = 3.79m
            }
        ];

        public static PagedList<StudentDto> MockPagedList = new()
        {
            Items = MockStudentsToReturn.ToList(),
            Page = 1,
            PageSize = 3,
            TotalCount = 3
        };

        public static List<StudentDto> MockStudentsToReturnAcrossMultiplePages =
        [
            new StudentDto()
            {
                SId = 4,
                Name = "Test student 4",
                GPA = 2.99m
            },
            new StudentDto()
            {
                SId = 5,
                Name = "Test student 5",
                GPA = 2.89m
            },
            new StudentDto()
            {
                SId = 6,
                Name = "Test student 6",
                GPA = 2.79m
            }
        ];

        public static PagedList<StudentDto> MockPagedListAcrossMultiplePages = new()
        {
            Items = MockStudentsToReturnAcrossMultiplePages.ToList(),
            Page = 2,
            PageSize = 3,
            TotalCount = 9
        };

        // For consumer
        public static dynamic ExpectedPagedStudentsJsonBody = new 
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

        public static dynamic ExpectedStudentsAcrossMultiplePagesJsonBody = new 
        {
            items = new List<object>()
            {
                        new
                        {
                            sId = Match.Equality(4),
                            name = Match.Equality("Test student 4"),
                            GPA = Match.Equality(2.99)
                        },
                        new
                        {
                            sId = Match.Equality(5),
                            name = Match.Equality("Test student 5"),
                            GPA = Match.Equality(2.89)
                        },
                        new
                        {
                            sId = Match.Equality(6),
                            name = Match.Equality("Test student 6"),
                            GPA = Match.Equality(2.79)
                        },
            },
            page = 2,
            pageSize = 3,
            totalCount = 9
        };

        public static readonly byte[] MockRowVersion = new byte[] { 0, 0, 3, 4, 5, 6, 7, 255 };
        public static readonly string MockRowVersionBase64Encoded = "AAADBAUGB/8=";
    }
}