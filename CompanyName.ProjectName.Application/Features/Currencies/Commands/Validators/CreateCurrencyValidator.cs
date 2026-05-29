namespace CompanyName.ProjectName.Application.Features.Currencies.Commands.Validators;

public sealed class CreateCurrencyValidator : AbstractValidator<CreateCurrencyCommand>
{
    public CreateCurrencyValidator()
    {
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