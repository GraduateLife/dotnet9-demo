using FluentValidation;

namespace WebApiProject.Todo.GetTodoByIdRequest;

public class Validator : AbstractValidator<GetTodoByIdRequest>
{
    public Validator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Todo Id must be a positive integer");
    }
}