var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

List<Todo> todos = [];
var currentId = 1;

app.MapGet("/todos", () => Results.Ok(todos));

app.MapGet("/todos/{id}", (int id) =>
{
    var res = todos.Find(t => t.Id == id);
    return res == null ? Results.NotFound($"Cannot find any todo with id {id}") : Results.Ok(res);
});

app.MapPost("/todos", (string title) =>
{
    if (todos.Any(t => t.Title == title))
    {
        return Results.BadRequest("Title already exists");
    }
    var toCreate=new Todo(currentId, title);
    todos.Add(toCreate);
    currentId++;
    return Results.Created($"/todos/{toCreate.Id}", toCreate);
});

app.MapPut("/todos", (Todo fromRequest) =>
{
    
    if (todos.Any(t => t.Title == fromRequest.Title))
    {
        return Results.BadRequest("Title already exists");
    }
    var toCreate=new Todo(currentId, fromRequest.Title, fromRequest.Description);
    todos.Add(toCreate);
    currentId++;
    return Results.Created($"/todos/{toCreate.Id}", toCreate);
});

app.MapDelete("/todos/{id}", (int id) =>
{
    var res = todos.Find(t => t.Id == id);
    if (res == null)
    {
        return Results.NotFound();
    }
    todos.Remove(res);
    return Results.NoContent();
});



app.Run();


internal class Todo(int id, string title, string description="")
{
    public int Id { get; set; } = id;
    public string Title { get; set; }=title;
    public string Description { get; set; }=description;
    public bool IsCompleted { get; set; }=false;
}
