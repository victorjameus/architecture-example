namespace CompanyName.ProjectName.Application.Features.Conversion.Commands.Validators;

public sealed class CreateConversionValidator : AbstractValidator<CreateConversionCommand>
{
    public CreateConversionValidator()
    {
        RuleFor(x => x.FromCurrencyId)
            .GreaterThan(0).WithMessage("La moneda origen es requerida.");

        RuleFor(x => x.ToCurrencyId)
            .GreaterThan(0).WithMessage("La moneda destino es requerida.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("El monto debe ser mayor a 0.");

        RuleFor(x => x)
            .Must(x => x.FromCurrencyId != x.ToCurrencyId)
            .WithMessage("La moneda origen y destino no pueden ser iguales.");
    }
}