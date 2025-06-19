using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApiProject.Todo.UpdateTodoTitleCommand;

public class UpdateTodoTitleHttpRequest
{
    public string Title { get; init; }
}

public static class TodoAppEndpoint
{
    public static IEndpointRouteBuilder AddUpdateTodoTitleEndpoint(this IEndpointRouteBuilder routeGroup)
    {
        routeGroup.MapPost("/u/{id:int}",
                async (
                    [FromRoute] int id,
                    [FromBody] UpdateTodoTitleHttpRequest fromRequest,
                    IMediator mediator, IMapper mapper, CancellationToken cancellationToken) =>
                {
                    var toSend = mapper.Map<UpdateTodoTitleCommand>(fromRequest);
                    toSend.Id = id;
                    return await mediator.Send(toSend, cancellationToken);
                })
            .WithName("UpdateTodoTitle");

        return routeGroup;
    }
}