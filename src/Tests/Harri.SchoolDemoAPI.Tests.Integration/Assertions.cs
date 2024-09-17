using FluentAssertions;
using Harri.SchoolDemoAPI.Models.Dto;
using System.Globalization;

namespace Harri.SchoolDemoAPI.Tests.Integration
{
    internal static class Assertions
    {

        public static void AssertInAscendingOrder(List<StudentDto> response, Func<StudentDto, object?> expectedColumnSelector) => AssertInOrder(response, expectedColumnSelector, ascending: true);

        // Assertion methods
        public static void AssertInAscendingOrder_BySId(List<StudentDto> response)
        {
            var ids = response.Select(x => x.SId).ToList();
            ids.Count.Should().BeGreaterThan(0);
            ids.Should().BeInAscendingOrder();
        }

        public static void AssertInDescendingOrder(List<StudentDto> response, Func<StudentDto, object?> expectedColumnSelector) => AssertInOrder(response, expectedColumnSelector, ascending: false);

        public static void AssertInOrder(List<StudentDto> response, Func<StudentDto, object?> expectedColumnSelector, bool ascending)
        {
            response.Should().NotBeEmpty();

            var values = response.Select(expectedColumnSelector).ToList();
            values.Count.Should().BeGreaterThan(1);

            if (values.First() is string) //Case insensitive string compare
            {
                var comparer = CultureInfo.InvariantCulture.CompareInfo.GetStringComparer(CompareOptions.IgnoreCase);

                if (ascending)
                {
                    values.Should().BeInAscendingOrder(x => (string?)x, comparer);
                }
                else
                {
                    values.Should().BeInDescendingOrder(x => (string?)x, comparer);
                }
            }
            else
            {
                if (ascending)
                {
                    values.Should().BeInAscendingOrder();
                }
                else
                {
                    values.Should().BeInDescendingOrder();
                }
            }
        }

        public static void AssertPageResponse(PagedList<StudentDto> response, int expectedPage = 1, int expectedPageSize = 10)
        {
            response.Items.Should().NotBeEmpty().And.HaveCount(expectedPageSize);
            response.PageSize.Should().Be(expectedPageSize);
            response.Page.Should().Be(expectedPage);
            response.TotalCount.Should().BeGreaterThanOrEqualTo(expectedPageSize);
        }

        public static void AssertEmptyPageResponse(PagedList<StudentDto> response, int expectedPage = 1, int expectedPageSize = 10)
        {
            response.Items.Should().BeEmpty();
            response.Page.Should().Be(expectedPage);
            response.PageSize.Should().Be(expectedPageSize);
            response.TotalCount.Should().Be(0);
        }
    }
}