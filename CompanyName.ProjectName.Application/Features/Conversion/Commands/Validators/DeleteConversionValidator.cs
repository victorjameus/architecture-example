namespace CompanyName.ProjectName.Application.Features.Conversion.Commands.Validators;

public sealed class DeleteConversionValidator : AbstractValidator<DeleteConversionCommand>
{
    public DeleteConversionValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0.");
    }
}