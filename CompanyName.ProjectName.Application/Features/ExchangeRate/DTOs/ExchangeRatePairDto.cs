namespace CompanyName.ProjectName.Application.Features.ExchangeRate.DTOs;

public record ExchangeRatePairDto
(
    string FromCurrency,
    string ToCurrency,
    decimal Rate
);