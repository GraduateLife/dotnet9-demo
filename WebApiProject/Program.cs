using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using WebApiProject.Data;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen();

var configuration = builder.Configuration;
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseSqlite(configuration.GetConnectionString("Default")));

 // builder.Services.Configure<JsonOptions>(option =>
 // {
 //     option.SerializerOptions.UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow;
 //     option.SerializerOptions.PropertyNameCaseInsensitive = false;
 //     option.SerializerOptions.PropertyNamingPolicy = null;
 // });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/todos", async (TodoDbContext db) =>
{
    var res=await db.Todos.OrderBy(t => t.Id).AsNoTracking().ToListAsync();
    return Results.Ok(res);
}).WithName("GetListedTodos");

app.MapGet("/todos/{id}", async (int id, TodoDbContext db) =>
{
    var res = await db.Todos.FirstOrDefaultAsync(t => t.Id == id);
    return res is null ? Results.NotFound($"Cannot find any todo with id {id}") : Results.Ok(res);
}).WithName("GetTodoById");

app.MapPost("/todos", async (string title, TodoDbContext db) =>
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

app.MapPut("/todos", async (Todo fromRequest, TodoDbContext db) =>
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

app.MapDelete("/todos/{id}", async (int id, TodoDbContext db) =>
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

app.MapDelete("/todos/purge", async (TodoDbContext db) =>
{
    db.Todos.RemoveRange(db.Todos);
    await db.SaveChangesAsync();
    return Results.Ok("All todos deleted");
}).WithName("PurgeTodos");



app.MapPut("/todos/isvalid", (TodoCreateDto fromRequest) =>
{
    var vRes = new List<ValidationResult>();
    var isValid=Validator.TryValidateObject(fromRequest, new ValidationContext(fromRequest),vRes,true);
    
    if (isValid) return Results.Ok("this is valid");
    
    var errors=vRes.ToDictionary(vr=>vr.MemberNames.First(),vr=>new[] {vr.ErrorMessage??"unknown v error"});
    return Results.ValidationProblem(errors);
    
}).WithName("CheckTodo");

app.Run();

