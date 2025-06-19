using FluentValidation;

namespace WebApiProject.Todo.ListTodosRequest;

public class ListTodosQueryValidator : AbstractValidator<ListTodosRequest>
{
    // 允许的排序字段，保持小写以便不区分大小写比较
    private static readonly string[] AllowedOrderByFields = ["title", "dueatutc", "createdat", "id"];

    // 允许的排序方向，保持小写
    private static readonly string[] AllowedOrdering = ["asc", "desc"];

    public ListTodosQueryValidator()
    {
        RuleFor(query => query.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page number must be at least 1.");

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, 100) // 假设每页大小合理范围
            .WithMessage("Page size must be between 1 and 100.");

        RuleFor(query => query.SearchTerm)
            .MaximumLength(200).When(x => x.SearchTerm != null)
            .WithMessage("Search term cannot exceed 200 characters.")
            .Matches(@"^[a-zA-Z0-9\s]*$")
            .WithMessage("Search term contains invalid characters.");

        // 验证 OrderByField：只有当 OrderByField 有值时才验证其是否在允许的列表中
        RuleFor(query => query.OrderByField)
            .Must(field => !string.IsNullOrWhiteSpace(field) &&
                           AllowedOrderByFields.Any(f => f.Equals(field, StringComparison.OrdinalIgnoreCase)))
            .When(query => !string.IsNullOrWhiteSpace(query.OrderByField))
            .WithMessage(
                $"'{nameof(ListTodosRequest.OrderByField)}' must be one of: {string.Join(", ", AllowedOrderByFields)}.");

        // 验证 Ordering：只有当 Ordering 有值时才验证其是否在允许的列表中
        RuleFor(query => query.Ordering)
            .Must(order => !string.IsNullOrWhiteSpace(order) &&
                           AllowedOrdering.Any(o => o.Equals(order, StringComparison.OrdinalIgnoreCase)))
            .When(query => !string.IsNullOrWhiteSpace(query.Ordering))
            .WithMessage($"'{nameof(ListTodosRequest.Ordering)}' must be 'asc' or 'desc'.");

        // 额外的业务规则：如果提供了 Ordering 但没有提供 OrderByField，则为无效
        RuleFor(query => query.Ordering)
            .Must((query, ordering) =>
                string.IsNullOrWhiteSpace(ordering) || !string.IsNullOrWhiteSpace(query.OrderByField))
            .WithMessage("If '{PropertyName}' is provided, '{nameof(Handler.OrderByField)}' must also be provided.")
            .When(query => !string.IsNullOrWhiteSpace(query.Ordering)); // 仅当 Ordering 被提供时才检查此规则
    }
}