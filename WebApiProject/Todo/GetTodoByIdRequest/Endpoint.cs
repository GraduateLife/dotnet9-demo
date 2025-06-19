using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApiProject.Todo.GetTodoByIdRequest;

public static class TodoAppEndpoint
{
    public static IEndpointRouteBuilder AddGetTodoByIdEndpoint(this IEndpointRouteBuilder routeGroup)
    {
        routeGroup.MapGet("/{id:int}",
                async (
                        [FromRoute] int id,
                        IMediator mediator, CancellationToken cancellationToken)
                    => await mediator.Send(new GetTodoByIdRequest { Id = id }, cancellationToken))
            .WithName("GetTodoById");

        return routeGroup;
    }
}