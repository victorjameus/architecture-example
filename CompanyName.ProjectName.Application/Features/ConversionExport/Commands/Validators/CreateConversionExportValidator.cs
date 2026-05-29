namespace CompanyName.ProjectName.Application.Features.ConversionExport.Commands.Validators;

public sealed class CreateConversionExportValidator : AbstractValidator<CreateConversionExportCommand>
{
    public CreateConversionExportValidator()
    {
        RuleFor(x => x)
            .Must(x => !(x.DateFrom.HasValue && x.DateTo.HasValue && x.DateFrom > x.DateTo))
            .WithMessage("La fecha de inicio no puede ser mayor a la fecha de fin.");
    }
}