using System.Collections.Generic;

namespace Harri.SchoolDemoAPI.Models.Dto
{
    public class PagedList<T>
    {
        public List<T> Items { get; set; } = new List<T>();

        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public bool HasNextPage => Page * PageSize < TotalCount;
        public bool HasPreviousPage => Page > 1;
    }
}
