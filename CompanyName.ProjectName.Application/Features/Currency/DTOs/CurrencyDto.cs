namespace CompanyName.ProjectName.Application.Features.Currency.DTOs;

public record CurrencyDto
(
    int Id,
    string Code,
    string Name,
    string Symbol,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);