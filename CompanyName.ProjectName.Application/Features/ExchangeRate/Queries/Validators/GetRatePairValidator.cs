namespace CompanyName.ProjectName.Application.Features.ExchangeRate.Queries.Validators;

public sealed class GetRatePairValidator : AbstractValidator<GetRatePairQuery>
{
    public GetRatePairValidator()
    {
        RuleFor(x => x.FromCurrency)
            .NotEmpty().WithMessage("La moneda origen es requerida.")
            .Length(3).WithMessage("El código de moneda debe tener 3 caracteres.");

        RuleFor(x => x.ToCurrency)
            .NotEmpty().WithMessage("La moneda destino es requerida.")
            .Length(3).WithMessage("El código de moneda debe tener 3 caracteres.");

        RuleFor(x => x)
            .Must(x => x.FromCurrency != x.ToCurrency)
            .WithMessage("La moneda origen y destino no pueden ser iguales.");
    }
}