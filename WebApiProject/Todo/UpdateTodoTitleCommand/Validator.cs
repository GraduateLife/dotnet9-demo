using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WebApiProject.Data;

namespace WebApiProject.Todo.UpdateTodoTitleCommand;

public class TodoUpdateTitleValidator : AbstractValidator<UpdateTodoTitleCommand>
{
    private readonly TodoDbContext _todoDb;

    public TodoUpdateTitleValidator(TodoDbContext dbContext)
    {
        _todoDb = dbContext;

        RuleFor(x => x.NewTitle)
            .NotEmpty()
            .WithMessage("Title is required")
            .Matches("^[a-zA-Z]+$")
            .WithMessage("Title must contain only alphanumeric characters")
            .MustAsync(UniqueTitleInDb)
            .WithMessage("oops, someone took your todo!");
    }

    private async Task<bool> UniqueTitleInDb(string userProvided, CancellationToken cancellationToken)
    {
        return !await _todoDb.Todos.AnyAsync(t => t.Title.ToLower() == userProvided.ToLower(), cancellationToken);
    }
}