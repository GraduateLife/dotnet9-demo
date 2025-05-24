using System.ComponentModel.DataAnnotations.Schema;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace WebApiProject.Data;

[Table("Cool todo app")]
public class Todo
{
    // [Key]
    [Column("TodoId")]
    public int Id { get; set; }
    
    public string Title { get; set; }
    
    public string Description { get; set; }
    public bool IsCompleted { get; set; }
    
    public DateTime StartAtUTC { get; set; }
    
    public DateTime CompleteAtUTC { get; set; }
    
    public DateTime DueAtUTC { get; set; }
    
    public DateTime CreatedAtUTC { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? LastModifiedAtUTC { get; set; }
    public string? LastModifiedBy { get; set; }
    
}

public record TodoCreateDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime? StartAtUTC { get; set; }=DateTime.UtcNow;
    public DateTime DueAtUTC { get; set; }
}

public class TodoCreateValidator : AbstractValidator<TodoCreateDto>
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
            .MustAsync(UniqueTRiteInDb)
            .WithMessage("oops, someone took your todo!");
        
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required")
            .MaximumLength(50)
            .WithMessage("Description must not exceed 50 characters");

        RuleFor(x => x.StartAtUTC)
            .LessThan(x => x.DueAtUTC).When(x => x.StartAtUTC.HasValue)
            .WithMessage("Start date must be earlier than the due date.");
        
        RuleFor(x => x.DueAtUTC)
            .NotEmpty().WithMessage("Due date is required");
    }

    async Task<bool> UniqueTRiteInDb(string userProvided,  CancellationToken cancellationToken)
    {
        return !await _todoDb.Todos.AnyAsync(t => t.Title.ToLower() == userProvided.ToLower(), cancellationToken: cancellationToken);
    }
}