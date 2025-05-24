using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using WebApiProject.Data;
using WebApiProject.Filters;

namespace WebApiProject.Endpoints;

static class TodoAppV1Endpoints
{
    public static void AddV1Endpoints(this IEndpointRouteBuilder app, string tagName)
    {
        var todosGroup = app.MapGroup("v1/todos")
            .WithTags(tagName);
        
        todosGroup.MapGet("/", async (TodoDbContext db) =>
        {
            var res=await db.Todos.OrderBy(t => t.Id).AsNoTracking().ToListAsync();
            return Results.Ok(res);
        }).WithName("GetListedTodos");

        todosGroup.MapGet("/{id}", async (int id, TodoDbContext db) =>
        {
            var res = await db.Todos.FirstOrDefaultAsync(t => t.Id == id);
            return res is null ? Results.NotFound($"Cannot find any todo with id {id}") : Results.Ok(res);
        }).WithName("GetTodoById");

        todosGroup.MapPost("/", async (string title, TodoDbContext db) =>
        {
            var found = await db.Todos.FirstOrDefaultAsync(t => t.Title == title);
            if (found is not null)
            {
                return Results.BadRequest("Title already exists");
            }
            var toCreate=new Todo()
            {
                Title = title,
                Description = string.Empty,
                IsCompleted = false
            };
            db.Todos.Add(toCreate);
            await db.SaveChangesAsync();
            return Results.CreatedAtRoute("GetTodoById",new {id=toCreate.Id}, toCreate);
        }).WithName("CreateTodoByTitle");

        todosGroup.MapPut("/", async (Todo fromRequest, TodoDbContext db) =>
        {
            var has = await db.Todos.AnyAsync(t => t.Title == fromRequest.Title);
            if (has)
            {
                return Results.BadRequest("Title already exists");
            }

            var toCreate = new Todo()
            {
                Title = fromRequest.Title,
                Description = fromRequest.Description,
                IsCompleted = false
            };
            db.Todos.Add(toCreate);
            await db.SaveChangesAsync();
            return Results.CreatedAtRoute("GetTodoById",new {id=toCreate.Id}, toCreate);
        }).WithName("CreateTodoFromBody");

        todosGroup.MapDelete("/{id}", async (int id, TodoDbContext db) =>
        {
            var foundItem = await db.Todos.FirstOrDefaultAsync(t => t.Id == id);
            if (foundItem is null)
            {
                return Results.NotFound();
            }
            db.Todos.Remove(foundItem);
            await db.SaveChangesAsync();
            return Results.NoContent();
        }).WithName("DeleteTodoById");

        todosGroup.MapDelete("/purge", async (TodoDbContext db) =>
        {
            db.Todos.RemoveRange(db.Todos);
            await db.SaveChangesAsync();
            return Results.Ok("All todos deleted");
        }).WithName("PurgeTodos");

        todosGroup.MapPut("/{id}", async (int id, Todo fromRequest, TodoDbContext db) =>
        {
            var toUpdate = await db.Todos.FirstOrDefaultAsync(t => t.Id == id);

            if (toUpdate is null)
            {
                return Results.NotFound($"Cannot find any todo with id {id}");
            }
    
            var foundSameTitle = await db.Todos.FirstOrDefaultAsync(t => t.Title == fromRequest.Title && t.Id != id);
            if (foundSameTitle is not null)
            {
                return Results.BadRequest("Title already exists for another todo.");
            }

            toUpdate.Title = fromRequest.Title;
            toUpdate.Description = fromRequest.Description;
            toUpdate.IsCompleted = fromRequest.IsCompleted;

            await db.SaveChangesAsync();
            return Results.Ok(toUpdate);
        }).WithName("UpdateTodo");

        todosGroup.MapPut("/isvalid", (TodoCreateDto fromRequest) =>
        {
            var vRes = new List<ValidationResult>();
            var isValid=Validator.TryValidateObject(fromRequest, new ValidationContext(fromRequest),vRes,true);
    
            if (isValid) return Results.Ok("this is valid");
    
            var errors=vRes.ToDictionary(vr=>vr.MemberNames.First(),vr=>new[] {vr.ErrorMessage??"unknown v error"});
            return Results.ValidationProblem(errors);
    
        }).WithName("CheckTodo");

        todosGroup.MapPut("/isvalid/fv", async (TodoCreateDto fromRequest, TodoCreateValidator validator) =>
        {
            var vRes=await validator.ValidateAsync(fromRequest);
            return vRes.IsValid switch
            {
                true => Results.Ok("this is valid"),
                _ => Results.ValidationProblem(vRes.ToDictionary())
            };
        }).WithName("CheckTodoByFV");

        todosGroup.MapPut("/isvalid/fv2", (TodoCreateDto fromRequest) => Results.Ok("this is valid"))
            .WithName("CheckTodoByFV2")
            .AddEndpointFilter<ValidationFilter<TodoCreateDto>>();
    }
}