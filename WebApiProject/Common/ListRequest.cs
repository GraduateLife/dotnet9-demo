namespace WebApiProject.Common;

public interface IListRequest
{
    int PageNumber { get; set; }

    int PageSize { get; set; }
}