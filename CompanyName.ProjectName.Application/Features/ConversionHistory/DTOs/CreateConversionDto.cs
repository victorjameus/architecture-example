namespace CompanyName.ProjectName.Application.Features.ConversionHistory.DTOs;

public record CreateConversionDto
(
    int FromCurrencyId,
    int ToCurrencyId,
    decimal Amount
);