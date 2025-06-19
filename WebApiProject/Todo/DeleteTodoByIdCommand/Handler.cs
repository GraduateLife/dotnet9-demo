using MediatR;
using Microsoft.EntityFrameworkCore;
using WebApiProject.Data;

namespace WebApiProject.Todo.DeleteTodoByIdCommand;

public class DeleteTodoByIdCommand : IRequest<IResult>
{
    public int Id { get; set; }
}

public class GetTodoByIdRequestHandler(TodoDbContext db) : IRequestHandler<DeleteTodoByIdCommand, IResult>


{
    public async Task<IResult> Handle(DeleteTodoByIdCommand commmand, CancellationToken cancellationToken)
    {
        var todo = await db.Todos.FirstOrDefaultAsync(t => t.Id == commmand.Id, cancellationToken);

        if (todo is null) return Results.NotFound();

        db.Todos.Remove(todo);
        await db.SaveChangesAsync(cancellationToken);
        return Results.NoContent();
    }
}