namespace CompanyName.ProjectName.Application.Features.Conversion.DTOs;

public record CreateConversionDto
(
    int FromCurrencyId,
    int ToCurrencyId,
    decimal Amount
);