using Harri.SchoolDemoAPI.Models.Enums;

namespace Harri.SchoolDemoAPI.Models
{
    public static class APIDefaults
    {
        public static class Query
        {
            public const int Page = 1;
            public const int PageSize = 10;

            public const SortOrder OrderBy = SortOrder.ASC;
            public const string SortColumn = APIConstants.Student.SId;
        }
    }
}
