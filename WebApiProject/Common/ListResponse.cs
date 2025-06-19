namespace WebApiProject.Common;

internal interface IListResponse<T>
{
    public int TotalCount { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalPages { get; }
    public bool HasPreviousPage { get; }
    public bool HasNextPage { get; }
    public List<T> Items { get; set; }
}

public class ListResponse<T> : IListResponse<T>
{
    private ListResponse(List<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
    }

    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
    public List<T> Items { get; set; }

    public static ListResponse<TResult> CreateFromListItems<TResult>(List<TResult> items, int totalCount,
        IListRequest request)
    {
        return new ListResponse<TResult>(items, totalCount, request.PageNumber, request.PageSize);
    }
}