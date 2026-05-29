namespace CompanyName.ProjectName.Application.Features.ExchangeRate.DTOs;

public record ExchangeRateDto
(
    string BaseCurrency,
    Dictionary<string, decimal> Rates
);