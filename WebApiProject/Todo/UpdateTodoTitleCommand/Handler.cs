using MediatR;
using WebApiProject.Data;

namespace WebApiProject.Todo.UpdateTodoTitleCommand;

// Commands
public class UpdateTodoTitleCommand : IRequest<IResult>
{
    public int Id { get; set; }
    public string NewTitle { get; set; }
}

// Command Handler
public class UpdateTodoTitleCommandHandler(TodoDbContext db) : IRequestHandler<UpdateTodoTitleCommand, IResult>


{
    public async Task<IResult> Handle(UpdateTodoTitleCommand command, CancellationToken cancellationToken)
    {
        var found = await db.Todos.FindAsync([command.Id], cancellationToken);

        if (found is null) return Results.NotFound();

        found.Title = command.NewTitle;

        await db.SaveChangesAsync(cancellationToken);

        return Results.Ok(found);
    }
}