namespace CompanyName.ProjectName.Application.Features.ExchangeRate.Queries.Validators;

public sealed class GetAllRatesValidator : AbstractValidator<GetAllRatesQuery>
{
    public GetAllRatesValidator()
    {
        RuleFor(x => x.BaseCurrency)
            .NotEmpty().WithMessage("La moneda base es requerida.")
            .Length(3).WithMessage("El código de moneda debe tener 3 caracteres.");
    }
}