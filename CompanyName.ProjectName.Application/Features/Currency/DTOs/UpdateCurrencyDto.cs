namespace CompanyName.ProjectName.Application.Features.Currency.DTOs;

public record UpdateCurrencyDto
(
    int Id,
    string Code,
    string Name,
    string Symbol
);