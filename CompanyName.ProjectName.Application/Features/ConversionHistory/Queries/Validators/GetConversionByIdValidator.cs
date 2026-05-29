namespace CompanyName.ProjectName.Application.Features.ConversionHistory.Queries.Validators;

public sealed class GetConversionByIdValidator : AbstractValidator<GetConversionByIdQuery>
{
    public GetConversionByIdValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0.");
    }
}