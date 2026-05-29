namespace CompanyName.ProjectName.Application.Features.Currencies.Queries.Validators;

public sealed class GetCurrencyByIdValidator : AbstractValidator<GetCurrencyByIdQuery>
{
    public GetCurrencyByIdValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0.");
    }
}