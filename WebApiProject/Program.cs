using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebApiProject.Behaviors;
using WebApiProject.Data;
using WebApiProject.Todo.CreateTodoCommand;
using WebApiProject.Todo.DeleteTodoByIdCommand;
using WebApiProject.Todo.GetTodoByIdRequest;
using WebApiProject.Todo.ListTodosRequest;
using WebApiProject.Todo.UpdateTodoTitleCommand;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var configuration = builder.Configuration;
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("Default"))
        .LogTo(Console.WriteLine, LogLevel.Information) // 输出到控制台，级别为 Information
        .EnableSensitiveDataLogging() // 启用敏感数据日志（如参数值），仅限开发环境！
        .EnableDetailedErrors()); // 启用详细错误信息));

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
        dbContext.Database.Migrate();
    }

    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGroup("/todos")
    .AddCreateTodoEndpoint()
    .AddListTodosEndpoint()
    .AddGetTodoByIdEndpoint()
    .AddUpdateTodoTitleEndpoint()
    .AddDeleteTodoByIdEndpoint();


app.Run();