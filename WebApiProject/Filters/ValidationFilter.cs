using FluentValidation;

namespace WebApiProject.Filters;

public class ValidationFilter<T> : IEndpointFilter where T : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var model = context.Arguments.OfType<T>().FirstOrDefault();

        if (model is null) return await next(context);

        var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();

        if (validator is null) return await next(context);

        var validationResult = await validator.ValidateAsync(model);

        if (!validationResult.IsValid) return Results.ValidationProblem(validationResult.ToDictionary());

        return await next(context);
    }
}