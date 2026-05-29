namespace CompanyName.ProjectName.Application.Features.ConversionHistory.Queries.Validators;

public sealed class GetAllConversionsValidator : AbstractValidator<GetAllConversionsQuery>
{
    public GetAllConversionsValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("La página debe ser mayor a 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("El tamaño de página debe ser mayor a 0.")
            .LessThanOrEqualTo(100).WithMessage("El tamaño de página no puede superar 100.");
    }
}