namespace DartRad.Models
{
    public class BasePaginatedAjaxRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; } = 8;
    }

    public class PaginatedAjaxResponse<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; } = 8;
        public int TotalRecords { get; set; }
        public List<T> Data { get; set; } 
    }
}
