using AutoMapper;
using MediatR;

namespace WebApiProject.Todo.ListTodosRequest;

internal record ListTodosHttpRequest
{
    public bool? IsCompleted { get; set; }
    public string? Search { get; set; }
    public string? F { get; set; } //field
    public string? O { get; set; } //order asc or desc
    public int? P { get; set; } //page number from 1
    public int? Size { get; set; } //page size
}



public static class TodoAppEndpoint
{
    public static IEndpointRouteBuilder AddListTodosEndpoint(this IEndpointRouteBuilder routeGroup)
    {
        routeGroup.MapGet("/",
                async ([AsParameters] ListTodosHttpRequest fromRequest,
                        IMediator mediator,
                        IMapper mapper,
                        CancellationToken cancellationToken)
                    =>
                {
                    var toSend = mapper.Map<ListTodosRequest>(fromRequest);
                    return await mediator.Send(toSend, cancellationToken);
                }
            )
            .WithName("ListTodos");

        return routeGroup;
    }
}