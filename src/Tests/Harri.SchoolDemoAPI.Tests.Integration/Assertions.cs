using FluentAssertions;
using Harri.SchoolDemoAPI.Models.Dto;
using System.Globalization;

namespace Harri.SchoolDemoAPI.Tests.Integration
{
    // Common assertion methods and assertion extensions
    public static class Assertions
    {
        private static void AssertInAscendingOrder(List<StudentDto> response, Func<StudentDto, object?> expectedColumnSelector) => AssertInOrder(response, expectedColumnSelector, ascending: true);

        private static void AssertInAscendingOrder_BySId(List<StudentDto> response)
        {
            var ids = response.Select(x => x.SId).ToList();
            ids.Count.Should().BeGreaterThan(0);
            ids.Should().BeInAscendingOrder();
        }

        private static void AssertInDescendingOrder(List<StudentDto> response, Func<StudentDto, object?> expectedColumnSelector) => AssertInOrder(response, expectedColumnSelector, ascending: false);

        private static void AssertInOrder(List<StudentDto> response, Func<StudentDto, object?> expectedColumnSelector, bool ascending)
        {
            response.Should().NotBeEmpty();

            var values = response.Select(expectedColumnSelector).ToList();
            values.Count.Should().BeGreaterThan(1);

            if (values.First() is string) 
            {
                //Case insensitive string compare, by default FluentAssertions BeInAscendingOrder/BeInDescendingOrder is not case insensitive
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

        private static void AssertPageResponse(PagedList<StudentDto> response, int expectedPage, int expectedPageSize)
        {
            response.Items.Should().NotBeEmpty().And.HaveCount(expectedPageSize);
            response.PageSize.Should().Be(expectedPageSize);
            response.Page.Should().Be(expectedPage);
            response.TotalCount.Should().BeGreaterThanOrEqualTo(expectedPageSize);
        }

        private static void AssertEmptyPageResponse(PagedList<StudentDto> response, int expectedPage, int expectedPageSize)
        {
            response.Items.Should().BeEmpty();
            response.Page.Should().Be(expectedPage);
            response.PageSize.Should().Be(expectedPageSize);
            response.TotalCount.Should().Be(0);
        }

        private static void AssertSingleItemPageResponse(PagedList<StudentDto> response, int expectedPage, int expectedPageSize)
        {
            response.Items.Should().NotBeEmpty().And.ContainSingle();
            response.Page.Should().Be(1);
            response.PageSize.Should().Be(10);
            response.TotalCount.Should().Be(1);
            response.HasNextPage.Should().BeFalse();
            response.HasPreviousPage.Should().BeFalse();
        }

        // Extensions using Should naming
        public static void ShouldBeInAscendingOrder(this IEnumerable<StudentDto> response, Func<StudentDto, object?> expectedColumnSelector) => AssertInAscendingOrder(response.ToList(), expectedColumnSelector);
        public static void ShouldBeAscendingOrderedBySId(this IEnumerable<StudentDto> response) => AssertInAscendingOrder_BySId(response.ToList());
        public static void ShouldBeInDescendingOrder(this IEnumerable<StudentDto> response, Func<StudentDto, object?> expectedColumnSelector) => AssertInDescendingOrder(response.ToList(), expectedColumnSelector);

        public static void ShouldHavePaginationData(this PagedList<StudentDto> response, int expectedPage = 1, int expectedPageSize = 10) => AssertPageResponse(response, expectedPage, expectedPageSize);
        public static void ShouldHaveEmptyPaginationData(this PagedList<StudentDto> response, int expectedPage = 1, int expectedPageSize = 10) => AssertEmptyPageResponse(response, expectedPage, expectedPageSize);
        public static void ShouldHaveSingleItemPaginationData(this PagedList<StudentDto> response, int expectedPage = 1, int expectedPageSize = 10) => AssertSingleItemPageResponse(response, expectedPage, expectedPageSize);
    }
}