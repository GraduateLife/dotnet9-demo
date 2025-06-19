using MediatR;
using Microsoft.EntityFrameworkCore;
using WebApiProject.Data;

namespace WebApiProject.Todo.GetTodoByIdRequest;

public class GetTodoByIdRequest : IRequest<IResult>
{
    public int Id { get; set; }
}

public class GetTodoByIdRequestHandler(TodoDbContext db) : IRequestHandler<GetTodoByIdRequest, IResult>


{
    public async Task<IResult> Handle(GetTodoByIdRequest request, CancellationToken cancellationToken)
    {
        var query = db.Todos.AsNoTracking();
        var found = await query.FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        return found is null ? Results.NotFound() : Results.Ok(found);
    }
}