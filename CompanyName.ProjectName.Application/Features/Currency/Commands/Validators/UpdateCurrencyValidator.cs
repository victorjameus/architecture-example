namespace CompanyName.ProjectName.Application.Features.Currency.Commands.Validators;

public sealed class UpdateCurrencyValidator : AbstractValidator<UpdateCurrencyCommand>
{
    public UpdateCurrencyValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("El código es requerido.")
            .Length(3).WithMessage("El código debe tener 3 caracteres.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

        RuleFor(x => x.Symbol)
            .NotEmpty().WithMessage("El símbolo es requerido.")
            .MaximumLength(10).WithMessage("El símbolo no puede superar los 10 caracteres.");
    }
}