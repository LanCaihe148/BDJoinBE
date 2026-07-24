

namespace BDJoinSN.Application.Models.Pagination
{
    public class PaginatedResult<T>
    {
        public PaginatedResult()
        {
            Items = new List<T>();
        }

        public PaginatedResult(List<T> items, int totalCount, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = totalCount;
            Items = items;
        }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public List<T> Items { get; set; }
    }
}
