namespace CompanyName.ProjectName.Application.Features.Currencies.DTOs;

public record CreateCurrencyDto
(
    string Code,
    string Name,
    string Symbol
);