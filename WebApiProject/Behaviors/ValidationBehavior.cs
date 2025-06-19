using FluentValidation;
using MediatR;

namespace WebApiProject.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any()) return await next(cancellationToken);

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

        if (failures.Count == 0) return await next(cancellationToken);

        if (typeof(TResponse) != typeof(IResult)) throw new ValidationException(failures);

        var errors = failures.Select(f => new { f.PropertyName, f.ErrorMessage }).ToList();
        return (TResponse)Results.ValidationProblem(errors.ToDictionary(e => e.PropertyName,
            e => new[] { e.ErrorMessage }));
    }
}