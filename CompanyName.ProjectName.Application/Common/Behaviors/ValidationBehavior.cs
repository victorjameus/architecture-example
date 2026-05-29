namespace CompanyName.ProjectName.Application.Common.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (!validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var errors = validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(e => e != null)
            .Select(e => e.ErrorMessage)
            .ToList();

        if (errors.Count != 0)
        {
            throw new ValidationException(string.Join(", ", errors));
        }

        return await next();
    }
}