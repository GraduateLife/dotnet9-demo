using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApiProject.Todo.CreateTodoCommand;

public class CreateTodoHttpRequest
{
    public string Title { get; init; }

    public string Description { get; init; }

    public DateTime? StartAtUtc { get; init; }

    public string Duration { get; init; }
}

public static class TodoAppEndpoint
{
    public static IEndpointRouteBuilder AddCreateTodoEndpoint(this IEndpointRouteBuilder routeGroup)
    {
        routeGroup.MapPost("/",
                async (
                    [FromBody] CreateTodoHttpRequest fromRequest,
                    IMediator mediator, IMapper mapper, CancellationToken cancellationToken) =>
                {
                    var command = mapper.Map<CreateTodoCommand>(fromRequest);
                    return await mediator.Send(command, cancellationToken);
                })
            .WithName("CreateTodo");

        return routeGroup;
    }
}