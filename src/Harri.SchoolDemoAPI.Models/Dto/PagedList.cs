using System;
using System.Collections.Generic;

namespace Harri.SchoolDemoAPI.Models.Dto
{
    public class PagedList<T>
    {
        public List<T> Items { get; set; } = new List<T>();

        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        // Returns zero when it cannot calculate a value
        public int TotalPageCount
        {
            get
            {
                try
                {
                    return (int)Math.Ceiling(((decimal)TotalCount) / PageSize);
                }
                catch (DivideByZeroException)
                {
                    return 0;
                }
            }
        }

        public bool HasNextPage => Page * PageSize < TotalCount;
        public bool HasPreviousPage => Page > 1;
    }
}
