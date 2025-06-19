using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WebApiProject.Data;

namespace WebApiProject.Todo.CreateTodoCommand;

public class TodoCreateValidator : AbstractValidator<CreateTodoCommand>
{
    private readonly TodoDbContext _todoDb;

    public TodoCreateValidator(TodoDbContext dbContext)
    {
        _todoDb = dbContext;

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .Matches("^[a-zA-Z]+$")
            .WithMessage("Title must contain only alphanumeric characters")
            .MustAsync(UniqueTitleInDb)
            .WithMessage("oops, someone took your todo!");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required")
            .MaximumLength(50)
            .WithMessage("Description must not exceed 50 characters");

        RuleFor(x => x.StartAtUtc)
            .LessThan(x => x.DueAtUtc)
            .WithMessage("Start date must be earlier than the due date.");

        RuleFor(x => x.DueAtUtc)
            .NotEmpty().WithMessage("Due date is required");
    }

    private async Task<bool> UniqueTitleInDb(string userProvided, CancellationToken cancellationToken)
    {
        return !await _todoDb.Todos.AnyAsync(t => t.Title.ToLower() == userProvided.ToLower(), cancellationToken);
    }
}