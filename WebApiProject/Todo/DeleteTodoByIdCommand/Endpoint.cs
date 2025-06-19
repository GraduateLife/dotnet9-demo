using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApiProject.Todo.DeleteTodoByIdCommand;

public static class TodoAppEndpoint
{
    public static IEndpointRouteBuilder AddDeleteTodoByIdEndpoint(this IEndpointRouteBuilder routeGroup)
    {
        routeGroup.MapDelete("/{id:int}",
                async (
                        [FromRoute] int id,
                        IMediator mediator, CancellationToken cancellationToken)
                    => await mediator.Send(new DeleteTodoByIdCommand { Id = id }, cancellationToken))
            .WithName("DeleteTodoById");

        return routeGroup;
    }
}