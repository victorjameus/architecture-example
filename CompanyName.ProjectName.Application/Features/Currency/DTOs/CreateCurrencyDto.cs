namespace CompanyName.ProjectName.Application.Features.Currency.DTOs;

public record CreateCurrencyDto
(
    string Code,
    string Name,
    string Symbol
);