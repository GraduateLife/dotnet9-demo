using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebApiProject.Common;
using WebApiProject.Data;

namespace WebApiProject.Todo.ListTodosRequest;

public class ListTodosRequest : IRequest<IResult>, IListRequest
{
    public bool IsCompleted { get; set; }
    public string? SearchTerm { get; set; }
    public string OrderByField { get; set; }
    public string Ordering { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

public record TodoListItem
{
    public int Id { get; set; }

    public string Title { get; set; }
}

public class ListTodosRequestHandler(TodoDbContext db, IMapper mapper) : IRequestHandler<ListTodosRequest, IResult>


{
    public async Task<IResult> Handle(ListTodosRequest request, CancellationToken cancellationToken)
    {
        var todoQuery = db.Todos.AsNoTracking();
        
        todoQuery = todoQuery
            .Where(t => t.IsCompleted == request.IsCompleted);

        if (request.SearchTerm is not null)
        {
            var searchTermLower = request.SearchTerm.ToLower();
            todoQuery = todoQuery.Where(t => t.Title.ToLower().Contains(searchTermLower) ||
                                             t.Description.ToLower().Contains(searchTermLower));
        }


        todoQuery = request.OrderByField.ToLower() switch
        {
            "title" => request.Ordering.ToLower() == "desc"
                ? todoQuery.OrderByDescending(t => t.Title)
                : todoQuery.OrderBy(t => t.Title),
            "dueatutc" => request.Ordering.ToLower() == "desc"
                ? todoQuery.OrderByDescending(t => t.DueAtUtc)
                : todoQuery.OrderBy(t => t.DueAtUtc),
            "id" => request.Ordering.ToLower() == "desc"
                ? todoQuery.OrderByDescending(t => t.Id)
                : todoQuery.OrderBy(t => t.Id),
            _ => todoQuery
        };

        var totalCount = await todoQuery.CountAsync(cancellationToken);
        var pageSize = request.PageSize;
        var pageNumber = request.PageNumber;


        var listedResult = await todoQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize).ProjectTo<TodoListItem>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        var response = ListResponse<TodoListItem>.CreateFromListItems(
            listedResult,
            totalCount,
            request
        );

        return Results.Ok(response);
    }
}