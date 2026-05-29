namespace CompanyName.ProjectName.Application.Features.Currencies.DTOs;

public record UpdateCurrencyDto
(
    int Id,
    string Code,
    string Name,
    string Symbol
);