using AutoMapper;
using MediatR;
using WebApiProject.Data;

namespace WebApiProject.Todo.CreateTodoCommand;

// Commands
public class CreateTodoCommand : IRequest<IResult>
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required DateTime StartAtUtc { get; init; }

    public required DateTime DueAtUtc { get; init; }
}

// Command Handler
public class CreateTodoCommandHandler(TodoDbContext db, IMapper mapper) : IRequestHandler<CreateTodoCommand, IResult>


{
    public async Task<IResult> Handle(CreateTodoCommand command, CancellationToken cancellationToken)
    {
        var toCreate = mapper.Map<Data.Todo>(command);
        await db.Todos.AddAsync(toCreate, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }
}