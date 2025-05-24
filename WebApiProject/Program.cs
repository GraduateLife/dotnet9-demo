using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WebApiProject.Data;
using WebApiProject.Endpoints;
using WebApiProject.Filters;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen();

var configuration = builder.Configuration;
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseSqlite(configuration.GetConnectionString("Default")));




builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI((options)=>options.SwaggerEndpoint($"/swagger/v1/swagger.json", $"Todo API V1"));
    
}

app.UseHttpsRedirection();

app.AddV1Endpoints("super cool todo app v1");

app.Run();

